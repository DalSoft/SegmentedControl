﻿﻿using System;
using Xamarin.Forms;
using SegmentedControl.FormsPlugin.iOS;
using Xamarin.Forms.Platform.iOS;
using UIKit;

[assembly: ExportRenderer(typeof(SegmentedControl.FormsPlugin.Abstractions.SegmentedControl), typeof(SegmentedControlRenderer))]
namespace SegmentedControl.FormsPlugin.iOS
{
    /// <summary>
    /// SegmentedControl Renderer
    /// </summary>
    public class SegmentedControlRenderer : ViewRenderer<Abstractions.SegmentedControl, UISegmentedControl>
    {
        UISegmentedControl nativeControl;

        protected override void OnElementChanged(ElementChangedEventArgs<Abstractions.SegmentedControl> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                // Instantiate the native control and assign it to the Control property with
                // the SetNativeControl method

                nativeControl = new UISegmentedControl();

                for (var i = 0; i < Element.Children.Count; i++)
                {
                    nativeControl.InsertSegment(Element.Children[i].Text, i, false);
                }

                nativeControl.Enabled = Element.IsEnabled;
                nativeControl.TintColor = Element.IsEnabled ? Element.TintColor.ToUIColor() : Element.DisabledColor.ToUIColor();
                SetSelectedTextColor();
                SetFont();

                nativeControl.SelectedSegment = Element.SelectedSegment;

                SetNativeControl(nativeControl);
            }

            if (e.OldElement != null)
            {
                // Unsubscribe from event handlers and cleanup any resources

                if (nativeControl != null)
                    nativeControl.ValueChanged -= NativeControl_ValueChanged;
            }

            if (e.NewElement != null)
            {
                // Configure the control and subscribe to event handlers

                nativeControl.ValueChanged += NativeControl_ValueChanged;
            }
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (nativeControl == null || Element == null) return;

            switch (e.PropertyName)
            {
                case "Renderer":
                    Element?.SendValueChanged();
                    break;
                case nameof(Element.SelectedSegment):
                    nativeControl.SelectedSegment = Element.SelectedSegment;
                    Element.SendValueChanged();
                    break;
                case nameof(Element.TintColor):
                    nativeControl.TintColor = Element.IsEnabled ? Element.TintColor.ToUIColor() : Element.DisabledColor.ToUIColor();
                    break;
                case nameof(Element.IsEnabled):
                    nativeControl.Enabled = Element.IsEnabled;
                    nativeControl.TintColor = Element.IsEnabled ? Element.TintColor.ToUIColor() : Element.DisabledColor.ToUIColor();
                    break;
                case nameof(Element.SelectedTextColor):
                    SetSelectedTextColor();
                    break;
                case nameof(Element.FontFamily):
                case nameof(Element.FontSize):
                    SetFont();
                    break;
            }

        }

        void SetSelectedTextColor()
        {
            var attr = new UITextAttributes
            {
                TextColor = Element.SelectedTextColor.ToUIColor()
            };
            nativeControl.SetTitleTextAttributes(attr, UIControlState.Selected);
        }

        void SetFont()
        {
            var font = UIFont.FromName(Element.FontFamily, Element.FontSize);

            if (font != null) // Attempting to set the font to null causes a crash
                nativeControl.SetTitleTextAttributes(new UITextAttributes { Font = font }, UIControlState.Normal);
        }

        void NativeControl_ValueChanged(object sender, EventArgs e)
        {
            Element.SelectedSegment = (int)nativeControl.SelectedSegment;
        }

        protected override void Dispose(bool disposing)
        {
            if (nativeControl != null)
            {
                nativeControl.ValueChanged -= NativeControl_ValueChanged;
                nativeControl.Dispose();
                nativeControl = null;
            }

            try
            {
                base.Dispose(disposing);
            }
            catch
            {
                return;
            }
        }

        /// <summary>
        /// Used for registration with dependency service
        /// </summary>
        public static void Init()
        {
            var temp = DateTime.Now;
        }
    }
}
