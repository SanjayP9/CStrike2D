using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CStrike2D;
using Microsoft.Xna.Framework.Graphics;

namespace cstrike2d
{
    public class UIManager
    {
        List<GUIComponent> guiComponents = new List<GUIComponent>();

        public void AddComponent(GUIComponent component)
        {
            guiComponents.Add(component);
        }

        public void Update(float gameTime)
        {
            foreach (GUIComponent component in guiComponents)
            {
                component.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch sb, Assets assets)
        {
            foreach (GUIComponent component in guiComponents)
            {
                component.Draw(sb, assets);
            }
        }

        /// <summary>
        /// Checks if a button was clicked
        /// </summary>
        /// <param name="identifier"></param>
        public bool Clicked(InputManager input, string identifier)
        {
            Button button = (Button)guiComponents.Find(btn => btn.Identifier == identifier);

            return button != null && button.Clicked(input);
        }

        public bool Hover(InputManager input, string identifier)
        {
            Button button = (Button) guiComponents.Find(btn => btn.Identifier == identifier);

            return button != null && button.Hover(input);
        }

        /// <summary>
        /// Shows the specified component
        /// </summary>
        /// <param name="identifier"></param>
        public void Show(string identifier)
        {
            GUIComponent component = guiComponents.Find(comp => comp.Identifier == identifier);

            // If the component is not null, show the component
            if (component != null)
            {
                component.Show();
            }
        }

        /// <summary>
        /// Hides the specified component
        /// </summary>
        /// <param name="identifier"></param>
        public void Hide(string identifier)
        {
            GUIComponent component = guiComponents.Find(comp => comp.Identifier == identifier);

            // If the component is not null, show the component
            if (component != null)
            {
                component.Hide();
            }
        }
    }
}
