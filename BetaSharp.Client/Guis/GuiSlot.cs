using BetaSharp.Client.Input;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Client.Rendering.Guis;
using Silk.NET.OpenGL.Legacy;

namespace BetaSharp.Client.Guis;

public abstract class GuiSlot
{
    private readonly Minecraft _mc;
    private readonly int _width;
    private readonly int _height;
    protected readonly int _top;
    protected readonly int _bottom;
    private readonly int _right;
    private readonly int _left;
    protected readonly int _posZ;

    private int _scrollUpButtonID;
    private int _scrollDownButtonID;
    private float _initialClickY = -2.0F;
    private float _scrollMultiplier;
    private float _amountScrolled;
    private int _selectedElement = -1;
    private long _lastClicked;

    private bool _showSelectionHighlight = true;
    private bool _hasHeader;
    private int _headerHeight;

    public GuiSlot(Minecraft mc, int width, int height, int top, int bottom, int posZ)
    {
        _mc = mc;
        _width = width;
        _height = height;
        _top = top;
        _bottom = bottom;
        _posZ = posZ;
        _left = 0;
        _right = width;
    }

    public void SetShowSelectionHighlight(bool value) => _showSelectionHighlight = value;


    protected void SetHeader(bool hasHeader, int headerHeight)
    {
        _hasHeader = hasHeader;
        _headerHeight = headerHeight;
        if (!hasHeader) _headerHeight = 0;
    }

    public abstract int GetSize();

    protected abstract void ElementClicked(int index, bool doubleClick);

    protected abstract bool IsSelected(int slotIndex);

    protected virtual int GetContentHeight() => GetSize() * _posZ + _headerHeight;

    protected abstract void DrawBackground();

    protected abstract void DrawSlot(int index, int x, int y, int height, GuiBatch batch);

    protected virtual void DrawHeader(int x, int y, GuiBatch batch) { }

    protected virtual void HeaderClicked(int var1, int var2)
    {
    }

    protected virtual void PostDrawScreen(int mouseX, int mouseY) { }

    public int GetSlotAt(int mouseX, int mouseY)
    {
        int minX = _width / 2 - 110;
        int maxX = _width / 2 + 110;
        int relativeY = mouseY - _top - _headerHeight + (int)_amountScrolled - 4;
        int index = relativeY / _posZ;

        return (mouseX >= minX && mouseX <= maxX && index >= 0 && relativeY >= 0 && index < GetSize())
            ? index
            : -1;
    }

    public void RegisterScrollButtons(List<GuiButton> buttons, int upId, int downId)
    {
        _scrollUpButtonID = upId;
        _scrollDownButtonID = downId;
    }

    private void BindAmountScrolled()
    {
        int maxScroll = GetContentHeight() - (_bottom - _top - 4);
        if (maxScroll < 0) maxScroll /= 2;

        if (_amountScrolled < 0.0f) _amountScrolled = 0.0f;
        if (_amountScrolled > maxScroll) _amountScrolled = maxScroll;

    }

    public void ActionPerformed(GuiButton button)
    {
        if (!button.Enabled) return;

        if (button.Id == _scrollUpButtonID)
        {
            _amountScrolled -= _posZ * 2 / 3;
            _initialClickY = -2.0f;
            BindAmountScrolled();
        }
        else if (button.Id == _scrollDownButtonID)
        {
            _amountScrolled += _posZ * 2 / 3;
            _initialClickY = -2.0f;
            BindAmountScrolled();
        }
    }

    public void DrawScreen(int mouseX, int mouseY, float partialTicks)
    {
        DrawBackground();

        int listSize = GetSize();
        int scrollbarXStart = _width / 2 + 124;
        int scrollbarXEnd = scrollbarXStart + 6;

        if (Mouse.isButtonDown(0))
        {
            if (_initialClickY == -1.0f)
            {
                bool shouldCaptureMouse = true;

                if (mouseY >= _top && mouseY <= _bottom)
                {
                    int contentMinX = _width / 2 - 110;
                    int contentMaxX = _width / 2 + 110;
                    int relativeY = mouseY - _top - _headerHeight + (int)_amountScrolled - 4;
                    int slotIndex = relativeY / _posZ;

                    if (mouseX >= contentMinX && mouseX <= contentMaxX && slotIndex >= 0 && relativeY >= 0 && slotIndex < listSize)
                    {
                        bool isDoubleClick = slotIndex == _selectedElement && (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
 - _lastClicked < 250L);
                        ElementClicked(slotIndex, isDoubleClick);
                        _selectedElement = slotIndex;
                        _lastClicked = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
;
                    }
                    else if (mouseX >= contentMinX && mouseX <= contentMaxX && relativeY < 0)
                    {
                        HeaderClicked(mouseX - contentMinX, mouseY - _top + (int)_amountScrolled - 4);
                        shouldCaptureMouse = false;
                    }

                    if (mouseX >= scrollbarXStart && mouseX <= scrollbarXEnd)
                    {
                        _scrollMultiplier = -1.0f;
                        int maxScroll = Math.Max(1, GetContentHeight() - (_bottom - _top - 4));
                        int viewHeight = _bottom - _top;
                        int barHeight = Math.Clamp((viewHeight * viewHeight) / GetContentHeight(), 32, viewHeight - 8);

                        _scrollMultiplier /= (float)(viewHeight - barHeight) / maxScroll;
                    }
                    else
                    {
                        _scrollMultiplier = 1.0f;
                    }

                    _initialClickY = shouldCaptureMouse ? mouseY : -2.0f;
                }
                else
                {
                    _initialClickY = -2.0f;
                }
            }
            else if (_initialClickY >= 0.0f)
            {
                _amountScrolled -= (mouseY - _initialClickY) * _scrollMultiplier;
                _initialClickY = mouseY;
            }
        }
        else
        {
            _initialClickY = -1.0f;
        }

        BindAmountScrolled();

        GLManager.GL.Disable(GLEnum.Lighting);
        GLManager.GL.Disable(GLEnum.Fog);
        var batch = _mc.guiBatch;

        var bgTex = _mc.textureManager.GetTextureId("/gui/background.png");
        _mc.textureManager.BindTexture(bgTex);
        GLManager.GL.Color4(1.0f, 1.0f, 1.0f, 1.0f);
        const float texSize = 256f;

        int scroll = (int)_amountScrolled;
        batch.DrawTexturedQuad(_left, _top, _right - _left, _bottom - _top,
            _left / texSize, (_bottom + scroll) / texSize,   // v flipped
            _right / texSize, (_top + scroll) / texSize,
            Color.FromArgb(0xFF606060), 0f, (uint)bgTex.Id);

        int startX = _width / 2 - 92 - 16;
        int startY = _top + 4 - (int)_amountScrolled;

        if (_hasHeader)
        {
            DrawHeader(startX, startY, batch);
        }

        for (int i = 0; i < listSize; ++i)
        {
            int slotY = startY + i * _posZ + _headerHeight;
            int slotHeight = _posZ - 4;

            if (slotY > _bottom || slotY + slotHeight < _top) continue;

            if (_showSelectionHighlight && IsSelected(i))
            {
                int minX = _width / 2 - 110;
                int maxX = _width / 2 + 110;
                GLManager.GL.Color4(1.0f, 1.0f, 1.0f, 1.0f);

                batch.DrawRect(minX, slotY - 2, maxX, slotY + slotHeight + 2, Color.FromArgb(0xFF808080), 0f);
                batch.DrawRect(minX + 1, slotY - 1, maxX - 1, slotY + slotHeight + 1, Color.FromArgb(0xFF000000), 0f);
            }

            DrawSlot(i, startX, slotY, slotHeight, batch);
        }

        GLManager.GL.Disable(GLEnum.DepthTest);
        OverlayBackground(0, _top, 255, 255);
        OverlayBackground(_bottom, _height, 255, 255);

        GLManager.GL.Enable(GLEnum.Blend);
        GLManager.GL.BlendFunc(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha);
        GLManager.GL.Disable(GLEnum.AlphaTest);
        GLManager.GL.ShadeModel(GLEnum.Smooth);

        // Top/Bottom gradient shadows
        const int shadowHeight = 4;
        batch.DrawGradientRect(_left, _right, _top, _top + shadowHeight, Color.FromArgb(0x00000000), Color.FromArgb(0xFF000000), 0f);
        batch.DrawGradientRect(_left, _right, _bottom - shadowHeight, _bottom, Color.FromArgb(0xFF000000), Color.FromArgb(0x00000000), 0f);

        // Scrollbar Rendering
        int scrollRange = GetContentHeight() - (_bottom - _top - 4);
        if (scrollRange > 0)
        {
            int viewHeight = _bottom - _top;
            int barHeight = Math.Clamp((viewHeight * viewHeight) / GetContentHeight(), 32, viewHeight - 8);
            int barY = (int)_amountScrolled * (viewHeight - barHeight) / scrollRange + _top;
            barY = Math.Max(barY, _top);

            batch.DrawRect(scrollbarXStart, _top, scrollbarXEnd, _bottom, Color.FromArgb(0xFF000000), 0f);
            batch.DrawRect(scrollbarXStart, barY, scrollbarXEnd, barY + barHeight, Color.FromArgb(0xFF808080), 0f);
            batch.DrawRect(scrollbarXStart, barY, scrollbarXEnd - 1, barY + barHeight - 1, Color.FromArgb(0xFFC0C0C0), 0f);
        }

        PostDrawScreen(mouseX, mouseY);

        GLManager.GL.Enable(GLEnum.Texture2D);
        GLManager.GL.ShadeModel(GLEnum.Flat);
        GLManager.GL.Enable(GLEnum.AlphaTest);
        GLManager.GL.Disable(GLEnum.Blend);
    }

    private void OverlayBackground(int startY, int endY, int alphaStart, int alphaEnd)
    {
        var topColor = Color.FromArgb((uint)((alphaStart << 24) | 0x404040));
        var bottomColor = Color.FromArgb((uint)((alphaEnd << 24) | 0x404040));
        _mc.guiBatch.DrawGradientRect(0, _width, startY, endY, topColor, bottomColor, 0f);
    }
}
