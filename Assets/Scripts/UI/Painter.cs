using System;
using System.Collections.Generic;
using Configs;
using Destruction;
using UnityEngine;

namespace UI
{
    public class Painter : MonoBehaviour
    {
        private const int Return = -1;
        private const int ColorPicker = -2;
        private const int ColorPickerWindow = -3;

        [SerializeField] private PainterConfiguration _configuration;
        [SerializeField] private RectTransform _colorsContent;
        [SerializeField] private RectTransform _windowsContent;
        [SerializeField] private RectTransform _wheelsContent;
        [SerializeField] private GameToggleButton _play;
        [SerializeField] private PainterToggleButton _colorsToggle;
        [SerializeField] private PainterToggleButton _windowsToggle;
        [SerializeField] private EventButton _matte;
        [SerializeField] private EventButton _glossy;
        [SerializeField] private EventButton _chrome;
        [SerializeField] private Screen _switcher;
        [SerializeField] private FlexibleColorPicker _colorPicker;
        [SerializeField] private GameObject _colorPickerHolder;
        [SerializeField] private EventButton _pickerClose;
        [SerializeField] private AudioSource _clickSound;

        private IPaintable _paintable;
        private Material _templateMaterial;
        private int _lastColor = -1;
        private int _pickerState;

        private void OnDestroy()
        {
            _colorPicker.onColorChange.RemoveListener(OnColorChange);
        }

        public void Initialize(Action onPlay)
        {
            _switcher.Initialize();
            _switcher.SetWindow((int)PainterWindow.Color);

            _templateMaterial = _configuration.MatteTemplate;
            _matte.Initialize(
                () =>
                {
                    _templateMaterial = _configuration.MatteTemplate;
                    OnColorClick(_lastColor);
                });
            _glossy.Initialize(
                () =>
                {
                    _templateMaterial = _configuration.GlossyTemplate;
                    OnColorClick(_lastColor);
                });
            _chrome.Initialize(
                () =>
                {
                    _templateMaterial = _configuration.ChromeTemplate;
                    OnColorClick(_lastColor);
                });

            GenerateCards(_configuration.Color, _configuration.Colors, _colorsContent, OnColorClick);
            GenerateCards(_configuration.Window, _configuration.Windows, _windowsContent, OnWindowClick);
            GenerateCards(_configuration.Wheels, _wheelsContent, OnWheelClick);

            _play.RegisterEvent(onPlay);
            _colorsToggle.RegisterEvent(CloseColorPicker);
            _windowsToggle.RegisterEvent(CloseColorPicker);
            _pickerClose.Initialize(CloseColorPicker);
            _colorPicker.onColorChange.AddListener(OnColorChange);
        }

        public void Activate(IPaintable paintable)
        {
            _paintable = paintable;
        }

        private void GenerateCards<T>(
            IReadOnlyList<SerializedPair<Sprite, T>> objects,
            Transform parent,
            Action<int> onClick)
        {
            Instantiate(_configuration.CarTemplate, parent)
                .Initialize(_configuration.Clear, Color.red, onClick, -1, _clickSound);

            for (int i = 0; i < objects.Count; i++)
            {
                Instantiate(_configuration.CarTemplate, parent)
                    .Initialize(objects[i].Key, Color.white, onClick, i, _clickSound);
            }
        }

        private void GenerateCards(
            Sprite sprite,
            IReadOnlyList<Color> objects,
            Transform parent,
            Action<int> onClick)
        {
            Instantiate(_configuration.CarTemplate, parent)
                .Initialize(_configuration.Clear, Color.red, onClick, -1, _clickSound);
            Instantiate(_configuration.CarTemplate, parent)
                .Initialize(_configuration.ColorPicker, Color.white, onClick, -2, _clickSound);

            for (int i = 0; i < objects.Count; i++)
                Instantiate(_configuration.CarTemplate, parent).Initialize(sprite, objects[i], onClick, i, _clickSound);
        }

        private void OnColorClick(int id)
        {
            _lastColor = id;

            switch (id)
            {
                case Return:
                    _paintable.ReturnPaint();
                    return;

                case ColorPicker:
                    ChangePickerVisibility(ColorPicker);
                    return;
            }

            CloseColorPicker();
            _paintable.PaintCar(CreatePaintMaterial(_configuration.Colors[id]));
        }

        private void OnWindowClick(int id)
        {
            switch (id)
            {
                case Return:
                    _paintable.ReturnWindows();
                    return;

                case ColorPicker:
                    ChangePickerVisibility(ColorPickerWindow);
                    return;
            }

            CloseColorPicker();
            _paintable.PaintWindows(CreateWindowMaterial(_configuration.Windows[id]));
        }

        private void OnWheelClick(int id)
        {
            if (id == Return)
            {
                _paintable.ReturnWheels();
                return;
            }

            Wheel wheel = _configuration.Wheels[id].Value;
            float size = _configuration.WheelScales[_paintable.WheelSize][wheel.Size];
            _paintable.ChangeWheels(wheel, size);
        }

        private void OnColorChange(Color color)
        {
            _lastColor = ColorPicker;

            switch (_pickerState)
            {
                case ColorPicker:
                    _paintable.PaintCar(CreatePaintMaterial(color));
                    break;

                case ColorPickerWindow:
                    _paintable.PaintWindows(CreateWindowMaterial(color));
                    break;
            }
        }

        private void ChangePickerVisibility(int state)
        {
            _pickerState = state;

            if (_colorPickerHolder.activeSelf == true)
            {
                _paintable.PaintCar(CreatePaintMaterial(_colorPicker.color));
                return;
            }

            _colorPickerHolder.SetActive(_colorPickerHolder.activeSelf == false);
        }

        private void CloseColorPicker()
        {
            _colorPickerHolder.SetActive(false);
        }

        private Material CreateWindowMaterial(Color color)
        {
            return new Material(_configuration.GlassTemplate)
            {
                color = new Color(color.r, color.g, color.b, _configuration.GlassTemplate.color.a)
            };
        }

        private Material CreatePaintMaterial(Color color)
        {
            return new Material(_templateMaterial)
            {
                color = color
            };
        }
    }
}