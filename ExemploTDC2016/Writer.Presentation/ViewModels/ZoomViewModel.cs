﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Waf.Applications;
using System.Windows.Input;
using Waf.Writer.Presentation.Services;

namespace Waf.Writer.Presentation.ViewModels
{
    public abstract class ZoomViewModel<T> : ViewModel<T>, IZoomCommands where T : IView
    {
        private const double minZoom = 0.2;
        private const double maxZoom = 16;

        private readonly IShellService shellService;
        private readonly ReadOnlyCollection<string> readOnlyDefaultZooms;
        private readonly DelegateCommand zoomInCommand;
        private readonly DelegateCommand zoomOutCommand;
        private readonly DelegateCommand fitToWidthCommand;
        private bool isVisible;
        private double zoom;


        protected ZoomViewModel(T view, IShellService shellService) : base(view)
        {
            this.shellService = shellService;
            readOnlyDefaultZooms = new ReadOnlyCollection<string>(new double[] { 2, 1.5, 1.25, 1, 0.75, 0.5 }
                .Select(d => string.Format(CultureInfo.CurrentCulture, "{0:P0}", d)).ToArray());
            zoomInCommand = new DelegateCommand(ZoomIn, CanZoomIn);
            zoomOutCommand = new DelegateCommand(ZoomOut, CanZoomOut);
            fitToWidthCommand = new DelegateCommand(FitToWidth);
            zoom = 1;
        }


        public IReadOnlyList<string> DefaultZooms { get { return readOnlyDefaultZooms; } }

        public ICommand ZoomInCommand { get { return zoomInCommand; } }

        public ICommand ZoomOutCommand { get { return zoomOutCommand; } }

        public ICommand FitToWidthCommand { get { return fitToWidthCommand; } }

        public bool IsVisible
        {
            get { return isVisible; }
            set
            {
                if (SetProperty(ref isVisible, value))
                {
                    if (isVisible) { shellService.ActiveZoomCommands = this; }
                    else { shellService.ActiveZoomCommands = null; }
                }
            }
        }

        public double Zoom
        {
            get { return zoom; }
            set
            {
                if (zoom != value)
                {
                    zoom = Math.Max(value, minZoom);
                    zoom = Math.Min(zoom, maxZoom);
                    RaisePropertyChanged();
                    zoomInCommand.RaiseCanExecuteChanged();
                    zoomOutCommand.RaiseCanExecuteChanged();
                }
            }
        }


        protected virtual void FitToWidthCore() { }
        
        private bool CanZoomIn() { return Zoom < maxZoom; }

        private void ZoomIn()
        {
            Zoom = Math.Floor(Math.Round((Zoom + 0.1) * 10, 3)) / 10;
        }

        private bool CanZoomOut() { return Zoom > minZoom; }

        private void ZoomOut()
        {
            Zoom = Math.Ceiling(Math.Round((Zoom - 0.1) * 10, 3)) / 10;
        }

        private void FitToWidth()
        {
            FitToWidthCore();
        }
    }
}
