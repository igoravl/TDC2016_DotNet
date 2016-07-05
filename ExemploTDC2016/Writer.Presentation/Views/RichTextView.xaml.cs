using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Waf.Applications;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Waf.Writer.Presentation.ViewModels;

namespace Waf.Writer.Presentation.Views
{
    [Export(typeof(IRichTextView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class RichTextView : UserControl, IRichTextView
    {
        private readonly Lazy<RichTextViewModel> viewModel;
        private IReadOnlyList<Control> dynamicContextMenuItems;
        private bool suppressTextChanged;


        public RichTextView()
        {
            InitializeComponent();

            viewModel = new Lazy<RichTextViewModel>(() => this.GetViewModel<RichTextViewModel>());
            Loaded += FirstTimeLoadedHandler;
            IsVisibleChanged += IsVisibleChangedHandler;
        }


        private RichTextViewModel ViewModel
        {
            get { return viewModel.Value; }
        }


        private void FirstTimeLoadedHandler(object sender, RoutedEventArgs e)
        {
            // Ensure that this handler is called only once.
            Loaded -= FirstTimeLoadedHandler;

            suppressTextChanged = true;
            richTextBox.Document = ViewModel.Document.Content;
            suppressTextChanged = false;
        }

        private void IsVisibleChangedHandler(object sender, DependencyPropertyChangedEventArgs e)
        {
            ViewModel.IsVisible = IsVisible;
        }

        private void RichTextBoxIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (richTextBox.IsVisible)
            {
                richTextBox.Focus();
            }
        }

        private void RichTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!suppressTextChanged)
            {
                ViewModel.Document.Modified = true;
            }

            UpdateFormattingProperties();
        }

        private void RichTextBoxSelectionChanged(object sender, RoutedEventArgs e)
        {
            UpdateFormattingProperties();
        }

        private void UpdateFormattingProperties()
        {
            TextRange selection = richTextBox.Selection;

            var fontWeight = selection.GetPropertyValue(TextElement.FontWeightProperty);
            var fontStyle = selection.GetPropertyValue(TextElement.FontStyleProperty);
            var textDecotations = selection.GetPropertyValue(Inline.TextDecorationsProperty);

            ViewModel.IsBold = fontWeight != DependencyProperty.UnsetValue &&
                               (FontWeight) fontWeight != FontWeights.Normal;
            ViewModel.IsItalic = fontStyle != DependencyProperty.UnsetValue &&
                                 (FontStyle) fontStyle == FontStyles.Italic;
            ViewModel.IsUnderline = textDecotations != DependencyProperty.UnsetValue &&
                                    textDecotations == TextDecorations.Underline;

            var isNumberedList = false;
            var isBulletList = false;
            ListItem listItem = null;
            if (selection.Start.Paragraph != null)
            {
                listItem = selection.Start.Paragraph.Parent as ListItem;
            }
            if (listItem != null)
            {
                var list = (List) listItem.Parent;
                isNumberedList = list.MarkerStyle == TextMarkerStyle.Decimal;
                isBulletList = list.MarkerStyle == TextMarkerStyle.Disc;
            }
            ViewModel.IsNumberedList = isNumberedList;
            ViewModel.IsBulletList = isBulletList;
        }

        private void RichTextBoxContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            var menuItems = new List<Control>();

            var spellingError = richTextBox.GetSpellingError(richTextBox.CaretPosition);
            if (spellingError != null)
            {
                foreach (var suggestion in spellingError.Suggestions.Take(5))
                {
                    var menuItem = new MenuItem
                    {
                        Header = suggestion,
                        FontWeight = FontWeights.Bold,
                        Command = EditingCommands.CorrectSpellingError,
                        CommandParameter = suggestion
                    };
                    menuItems.Add(menuItem);
                }

                if (!menuItems.Any())
                {
                    var noSpellingSuggestions = new MenuItem
                    {
                        Header = Properties.Resources.NoSpellingSuggestions,
                        FontWeight = FontWeights.Bold,
                        IsEnabled = false
                    };
                    menuItems.Add(noSpellingSuggestions);
                }

                menuItems.Add(new Separator());

                var ignoreAllMenuItem = new MenuItem
                {
                    Header = Properties.Resources.IgnoreAllMenu,
                    Command = EditingCommands.IgnoreSpellingError
                };
                menuItems.Add(ignoreAllMenuItem);

                menuItems.Add(new Separator());
            }

            foreach (var item in menuItems.Reverse<Control>())
            {
                contextMenu.Items.Insert(0, item);
            }

            dynamicContextMenuItems = menuItems;
        }

        private void RichTextBoxContextMenuClosing(object sender, ContextMenuEventArgs e)
        {
            foreach (var menuItem in dynamicContextMenuItems)
            {
                contextMenu.Items.Remove(menuItem);
            }
        }
    }
}