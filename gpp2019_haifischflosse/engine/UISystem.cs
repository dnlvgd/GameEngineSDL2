using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using XnaGeometry;
using static SDL2.SDL;

namespace gpp2019_haifischflosse
{
    public class UISystem
    {
        private List<TextComponent> textCPs = new List<TextComponent>();
        private List<ButtonClickAreaComponent> buttonClickAreaCps = new List<ButtonClickAreaComponent>();

        private Texture rectangleColliderTexture = new Texture("assets/sprites/collider/rectangleCollider.png");
        private Texture circleColliderTexture = new Texture("assets/sprites/collider/circleCollider.png");

        public TextComponent CreateTextComponent(string text, int fontSize, byte r, byte g, byte b, byte alpha)
        {
            TextComponent tc = new TextComponent(text, fontSize, r, g, b, alpha);
            textCPs.Add(tc);
            return tc;
        }

        public ButtonClickAreaComponent CreateButtonClickAreaComponent(int ownerTopLeftPosX, int ownerTopLeftPosY, int w, int h)
        {
            ButtonClickAreaComponent bcac = new ButtonClickAreaComponent(ownerTopLeftPosX, ownerTopLeftPosY, w, h);
            buttonClickAreaCps.Add(bcac);
            return bcac;
        }

        public void Render()
        {

            for (int i = 0; i < textCPs.Count; i++)
            {
                if (textCPs[i] == null)
                {
                    textCPs.RemoveAt(i);
                    i -= 1;
                    break;
                }

                if (textCPs[i].Owner.IsActive)
                {
                    var textSurface = textCPs[i].TextSurface;
                    var textSurfaceStructure = Marshal.PtrToStructure<SDL_Surface>(textSurface);
                    SDL_Rect textRectangle;
                    textRectangle.x = Convert.ToInt32(textCPs[i].Owner.Position.X);
                    textRectangle.y = Convert.ToInt32(textCPs[i].Owner.Position.Y);
                    textRectangle.w = (int)(textSurfaceStructure.w * Window.SCALEFACTOR_X);
                    textRectangle.h = (int)(textSurfaceStructure.h * Window.SCALEFACTOR_Y);

                    var textTexture = textCPs[i].TextTexture;

                    SDL_RenderCopy(Renderer.renderer, textTexture, System.IntPtr.Zero, ref textRectangle);
                }
            }
        }

        public void CheckAllButtonsForClickEvent(Vector2 mousePosition)
        {
            for (int i = buttonClickAreaCps.Count - 1; i >= 0; i--)
            {
                if (buttonClickAreaCps[i] == null || buttonClickAreaCps[i].Owner == null)
                {
                    continue;
                }

                if (IsButtonClicked(buttonClickAreaCps[i], mousePosition))
                {
                    EventSystem.Instance.AddEvent(buttonClickAreaCps[i].Owner.Name);
                    if (Window.isDebug)
                    {
                        Console.WriteLine("Clicked Button '" + buttonClickAreaCps[i].Owner.Name + "' + added event"); // its Name as 
                    }
                }
            }
        }

        public void Free()
        {
            for (int i = textCPs.Count - 1; i >= 0; i--)
            {
                if (textCPs[i].Owner.IsActive)
                {
                    TextComponent tc = textCPs[i].Owner.GetComponent<TextComponent>() as TextComponent;
                    if (tc == null)
                    {
                        continue;
                    }

                    if (tc.TextSurface != null && tc.TextSurface != IntPtr.Zero)
                    {
                        SDL_FreeSurface(tc.TextSurface);
                    }
                    if (tc.TextTexture != null && tc.TextTexture != IntPtr.Zero)
                    {
                        SDL_DestroyTexture(tc.TextTexture);
                    }
                }
            }
        }

        public void UpdateText(GameObject owner, string msg)
        {
            TextComponent tc = owner.GetComponent<TextComponent>() as TextComponent;
            if (tc != null)
            {
                tc.SetText(msg);
            }
        }

        private bool IsButtonClicked(ButtonClickAreaComponent bcac, Vector2 mousePosition)
        {
            //If the mouse is over the button
            if ((mousePosition.X >= bcac.ClickableBox.x) &&
                (mousePosition.X <= bcac.ClickableBox.x + bcac.ClickableBox.w) &&
                (mousePosition.Y >= bcac.ClickableBox.y) &&
                (mousePosition.Y <= bcac.ClickableBox.y + bcac.ClickableBox.h))
            {
                return true;
            }
            return false;
        }

        public void RescaleCollider()
        {
            foreach (var bc in buttonClickAreaCps)
            {
                bc.ClickableBox = new SDL_Rect() { x = (int)(bc.ClickableBox.x * Window.WINDOW_SCALEFACTOR_X), y = (int)(bc.ClickableBox.y * Window.WINDOW_SCALEFACTOR_Y), w = (int)(bc.ClickableBox.w * Window.WINDOW_SCALEFACTOR_X), h = (int)(bc.ClickableBox.h * Window.WINDOW_SCALEFACTOR_Y) };
            }
        }

        public void RenderColliderForDebug()
        {
            foreach (var bc in buttonClickAreaCps)
            {
                EventSystem.Instance.AddEvent("RenderOnce", rectangleColliderTexture, bc.ClickableBox.x, bc.ClickableBox.y, bc.ClickableBox.w, bc.ClickableBox.h);
            }
        }
    }
}
