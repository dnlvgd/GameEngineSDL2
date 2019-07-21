using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XnaGeometry;
using static SDL2.SDL;

namespace gpp2019_haifischflosse
{
    public class RenderSystem
    {
        //Save Renderer here
        private List<SpriteComponent> spriteCps = new List<SpriteComponent>();
        private List<TextureComponent> textureCps = new List<TextureComponent>();
        private int indexofLastEvent = 0;

        public SpriteComponent CreateSpriteComponent(Animation animation, string path, uint quantitySpriteRow, uint quantitySpriteColumn, uint totalQuantitySprites, int z, double animationDuration = 1000.0, SDL_RendererFlip rendererFlip = SDL_RendererFlip.SDL_FLIP_NONE)
        {
            SpriteComponent sc = new SpriteComponent(animation, path, quantitySpriteRow, quantitySpriteColumn, totalQuantitySprites, z, animationDuration, rendererFlip);
            spriteCps.Add(sc);
            return sc;
        }
        public TextureComponent CreateTextureComponent(string path, int z)
        {
            TextureComponent tc = new TextureComponent(path, z);
            textureCps.Add(tc);
            return tc;

        }
        public TextureComponent CreateTextureComponent(Texture tileTexture, SDL_Rect tileRect, int z)
        {
            TextureComponent tc = new TextureComponent(tileTexture, tileRect, z);
            textureCps.Add(tc);
            return tc;
        }
        public TextureComponent CreateTextureComponent(string path, int z, int w, int h)
        {
            TextureComponent tc = new TextureComponent(path, z, w, h);
            textureCps.Add(tc);
            return tc;

        }
        public void RenderOnce(Texture t, int x, int y, int w, int h)
        {
            SDL_Rect renderRect = new SDL_Rect();
            renderRect.x = x - Camera.camera.x;
            renderRect.y = y - Camera.camera.y;
            renderRect.w = w;
            renderRect.h = h;

            SDL_RenderCopyEx(Renderer.renderer, t.Textur, IntPtr.Zero, ref renderRect, 0, IntPtr.Zero, SDL_RendererFlip.SDL_FLIP_NONE);
        }

        public void Render(double deltaTime)
        {
            //Render renderTexture and renderRect
            Texture renderTexture = null;
            SDL_Rect renderRect = new SDL_Rect();

            //Render TextureComponent
            for (int i = 0; i < textureCps.Count; i++)
            {
                if (textureCps[i] == null)
                {
                    textureCps.RemoveAt(i);
                    i -= 1;
                    break;
                }

                if (textureCps[i].Owner.IsActive)
                {
                    renderTexture = textureCps[i].T;
                    renderRect.x = (int)textureCps[i].Owner.Position.X - Camera.camera.x;
                    renderRect.y = (int)textureCps[i].Owner.Position.Y - Camera.camera.y;
                    renderRect.w = (int)(textureCps[i].DstRect.w * Window.SCALEFACTOR_X * textureCps[i].Owner.Scaling.X);
                    renderRect.h = (int)(textureCps[i].DstRect.h * Window.SCALEFACTOR_Y * textureCps[i].Owner.Scaling.Y);

                    SDL_Rect textureClip = textureCps[i].DstRect;
                    var myClip = textureClip;
                    SDL_RenderCopyEx(Renderer.renderer, renderTexture.Textur, ref myClip, ref renderRect, textureCps[i].Owner.Angle, IntPtr.Zero, SDL_RendererFlip.SDL_FLIP_NONE);

                }

            }

            //Render SpriteComponent
            for (int i = 0; i < spriteCps.Count; i++)
            {
                if (spriteCps[i] == null)
                {
                    spriteCps.RemoveAt(i);
                    i -= 1;
                    continue;
                }

                if (spriteCps[i].Owner.IsActive && spriteCps[i].IsActive)
                {
                    renderTexture = spriteCps[i].S.Texture;

                    //calculate the animation frame that should be shown based on the delay between two animation frames
                    uint pastFrames = (uint)(spriteCps[i].S.AnimationTime / spriteCps[i].S.DelayBtwnFrames);
                    uint targetFrame = pastFrames % spriteCps[i].S.TotalQuantitySprites;
                    SDL_Rect spriteClip = spriteCps[i].S.SpriteClips[targetFrame];
                    
                    //go to next animation frame by increasing the internal animation time
                    spriteCps[i].S.AnimationTime += deltaTime;

                    //cycle animation
                    if (targetFrame >= spriteCps[i].S.TotalQuantitySprites)
                    {
                        spriteCps[i].S.AnimationTime = 0;
                    }

                    //offset & scale render dst rect
                    //renderRect.x = (int)spriteCps[i].Owner.Position.X - Camera.camera.x - (int)((spriteClip.w) * Window.SCALEFACTOR_X);//* spriteCps[i].Owner.Scaling.X);
                    renderRect.x = (int)spriteCps[i].Owner.Position.X - Camera.camera.x - (int)((spriteClip.w / 2) * Window.SCALEFACTOR_X);
                    //renderRect.y = (int)spriteCps[i].Owner.Position.Y - Camera.camera.y - (int)((spriteClip.h) * Window.SCALEFACTOR_Y);// * spriteCps[i].Owner.Scaling.Y);
                    renderRect.y = (int)spriteCps[i].Owner.Position.Y - Camera.camera.y - (int)((spriteClip.h / 2) * Window.SCALEFACTOR_Y);
                    renderRect.w = (int)(spriteClip.w * Window.SCALEFACTOR_X * spriteCps[i].Owner.Scaling.X);
                    renderRect.h = (int)(spriteClip.h * Window.SCALEFACTOR_Y * spriteCps[i].Owner.Scaling.Y);

                    var myClip = spriteClip;
                    SDL_RenderCopyEx(Renderer.renderer, renderTexture.Textur, ref myClip, ref renderRect, spriteCps[i].Owner.Angle, IntPtr.Zero, spriteCps[i].S.RendererFlip);

                    if (Window.isDebug)
                    {
                        SDL_RenderDrawPoint(Renderer.renderer, (int)spriteCps[i].Owner.Position.X, (int)spriteCps[i].Owner.Position.Y);
                    }
                }
            }
        }
        public void Update()
        {
            /* Event handling */
            List<EventStruct> myEvents;
            myEvents = EventSystem.Instance.GetEvents("AddSpriteCP", indexofLastEvent);
            foreach (EventStruct e in myEvents)
            {
                spriteCps.Add((SpriteComponent)e.data[0]);
            }
            myEvents = EventSystem.Instance.GetEvents("AddTextureCP", indexofLastEvent);
            foreach (EventStruct e in myEvents)
            {
                textureCps.Add((TextureComponent)e.data[0]);
            }
            myEvents = EventSystem.Instance.GetEvents("RemoveSpriteCP", indexofLastEvent);
            foreach (EventStruct e in myEvents)
            {
                spriteCps.Remove((SpriteComponent)e.data[0]);
            }
            myEvents = EventSystem.Instance.GetEvents("RemoveTextureCP", indexofLastEvent);
            foreach (EventStruct e in myEvents)
            {
                textureCps.Remove((TextureComponent)e.data[0]);
            }

            /* Sort textureCPs and spriteCPs Lists */
            SortList();
            indexofLastEvent = EventSystem.Instance.GetMyIndex();
        }
        public void RenderOnceEventHandling()
        {
            List<EventStruct> myEvents = EventSystem.Instance.GetEvents("RenderOnce", indexofLastEvent);
            foreach (EventStruct e in myEvents)
            {
                RenderOnce((Texture)e.data[0], (int)e.data[1], (int)e.data[2], (int)e.data[3], (int)e.data[4]);
            }
        }
        private void SortList()
        {
            spriteCps = spriteCps.OrderBy(x => x.Z).ToList();
            textureCps = textureCps.OrderBy(x => x.Z).ToList();
        }
        public void PlayAnimation(string goName, Animation targetAnimation)
        {
            // activate (play) the requested animation (deactivate all others) of a specific gameobject
            for (int i = 0; i < spriteCps.Count; i++)
            {
                if (SpriteComponentNotNull(spriteCps[i]))
                {
                    if(spriteCps[i].Owner.Name.Equals(goName))
                    {
                        if (spriteCps[i].Animation == targetAnimation)
                        {
                            spriteCps[i].IsActive = true;
                        }
                        else
                        {
                            //ChangeStartFrameOfAnimation(1, spriteCps[i]);
                            spriteCps[i].S.AnimationTime = 0;
                            spriteCps[i].IsActive = false;
                        }
                    }
                }
            }
        }
        private void ChangeStartFrameOfAnimation(uint startFrame, SpriteComponent sc)
        {
            if (SpriteComponentNotNull(sc))
            {
                uint frameStartIndex = startFrame - 1;
                int lastPossibleFrame = sc.S.SpriteClips.Length - 1;
                if (frameStartIndex >= 0 && frameStartIndex <= lastPossibleFrame)
                {
                    sc.S.CurrentSpriteClipIndex = frameStartIndex;
                }
            }
        }
        private bool SpriteComponentNotNull(SpriteComponent sc)
        {
            if (sc != null)
            {
                return true;
            }
            return false;
        }
    }
}
