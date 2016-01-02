using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace CStrike2D
{
    public class UIManager
    {
        private List<GUIComponent> guiComponents = new List<GUIComponent>();

        /// <summary>
        /// Adds a component to the list
        /// </summary>
        /// <param name="component"> The component to be added</param>
        public void AddComponent(GUIComponent component)
        {
            // Check if there are any duplicate identifiers before adding the component in. If there is, alert the developer
            if (!guiComponents.Exists(comp => comp.Identifier == component.Identifier))
            {
                guiComponents.Add(component);
            }
            else
            {
                throw new Exception("You tried adding a component with an identifier that already exists in the list." +
                    "The conflicting component name is: " + component.Identifier);
            }
        }

        /// <summary>
        /// Updates each component in the list
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(float gameTime)
        {
            foreach (GUIComponent component in guiComponents)
            {
                component.Update(gameTime);
            }
        }

        /// <summary>
        /// Draws the components
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="assets"></param>
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

        /// <summary>
        /// Hides all components associated with this manager
        /// </summary>
        public void HideAll()
        {
            foreach (GUIComponent component in guiComponents)
            {
                component.Hide();
            }
        }
    }
}
