using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace CStrike2D
{
    public class UIManager
    {
        private List<GUIComponent> guiComponents = new List<GUIComponent>();
        public List<GUIPage> GUIPages { get; private set; }

        public UIManager()
        {
            GUIPages = new List<GUIPage>();
        }

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
        /// Creates a new page which allows separation between sets of GUIComponents
        /// </summary>
        /// <param name="pageName"></param>
        public void FormPage(string pageName)
        {
            // Create a new page
            GUIPages.Add(new GUIPage(pageName, guiComponents));

            // Clear the "buffer" of GUIComponents
            guiComponents.Clear();
        }

        /// <summary>
        /// Updates all pages associated with this manager
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(float gameTime)
        {
            foreach (GUIPage page in GUIPages)
            {
                page.Update(gameTime);
            }
        }

        /// <summary>
        /// Draws all pages associated with this manager
        /// </summary>
        /// <param name="sb"></param>
        public void Draw(SpriteBatch sb)
        {
            foreach (GUIPage page in GUIPages)
            {
                page.Draw(sb);
            }
        }

        /// <summary>
        /// Draws a specific page
        /// </summary>
        /// <param name="pageName"></param>
        public void DrawPage(string pageName, SpriteBatch sb)
        {
            GUIPage page = GUIPages.Find(pg => pg.Identifier == pageName);

            if (page != null)
            {
                page.Draw(sb);
            }
        }

        /// <summary>
        /// Checks if a button was clicked
        /// </summary>
        /// <param name="input"></param>
        /// <param name="pageName"></param>
        /// <param name="identifier"></param>
        public bool Clicked(InputManager input, string pageName, string identifier)
        {
            GUIPage page = GUIPages.Find(pg => pg.Identifier == pageName);

            if (page != null)
            {
                return page.Clicked(input, identifier);
            }
            throw new NullReferenceException(
                "Attempted to access a page that does not exist. You tried to access a page named " +
                "\"" + pageName + "\"");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="pageName"></param>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public bool Hover(InputManager input, string pageName, string identifier)
        {
            GUIPage page = GUIPages.Find(pg => pg.Identifier == pageName);

            if (page != null)
            {
                return page.Hover(input, identifier);
            }
            throw new NullReferenceException(
                "Attempted to access a page that does not exist. You tried to access a page named " +
                "\"" + pageName + "\"");
        }

        /// <summary>
        /// Shows the specified component
        /// </summary>
        /// <param name="pageName"></param>
        /// <param name="identifier"></param>
        public void Show(string pageName, string identifier)
        {
            GUIPage page = GUIPages.Find(pg => pg.Identifier == pageName);

            // If the component is not null, show the component
            if (page != null)
            {
                page.Show(identifier);
            }
            else
            {
                throw new NullReferenceException(
                    "Attempted to show a page that does not exist. You tried to access a page named " +
                    "\"" + pageName + "\"");
            }
        }

        /// <summary>
        /// Shows a page
        /// </summary>
        /// <param name="pageName"></param>
        public void ShowPage(string pageName)
        {
            GUIPage page = GUIPages.Find(pg => pg.Identifier == pageName);

            if (page != null)
            {
                page.ShowAll();
            }
            else
            {
                throw new NullReferenceException("Attempted to show a page that does not exist. The " +
                                                 "page you tried to access was named \"" + pageName + "\"");
            }
        }

        /// <summary>
        /// Hides the specified component
        /// </summary>
        /// <param name="pageName"></param>
        /// <param name="identifier"></param>
        public void Hide(string pageName, string identifier)
        {
            GUIPage page = GUIPages.Find(pg => pg.Identifier == pageName);

            if (page != null)
            {
                page.Hide(identifier);
            }
            else
            {
                throw new NullReferenceException("Attempted to hide a component in a page that does not exist. The " +
                                                 "page you tried to access was named \"" + pageName + "\"");
            }
        }

        /// <summary>
        /// Hides a specific page
        /// </summary>
        /// <param name="pageName"></param>
        public void HidePage(string pageName)
        {
            GUIPage page = GUIPages.Find(pg => pg.Identifier == pageName);

            if (page != null)
            {
                page.HideAll();
            }
            else
            {
                throw new NullReferenceException("Attempted to hide a page that does not exist. The " +
                                                 "page you tried to access was named \"" + pageName + "\"");
            }
        }

        /// <summary>
        /// Hides all pages associated with this manager
        /// </summary>
        public void HideAll()
        {
            foreach (GUIPage page in GUIPages)
            {
                page.HideAll();
            }
        }
    }

    public class GUIPage
    {
        /// <summary>
        /// Identifier associated with this page
        /// </summary>
        public string Identifier { get; private set; }
        private List<GUIComponent> components; 

        /// <summary>
        /// Creates a set of components associated with an identifier for easy reference
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="components"></param>
        public GUIPage(string identifier, List<GUIComponent> components)
        {
            Identifier = identifier;
            this.components = components.ToList();
        }

        /// <summary>
        /// Updates all components in this page
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(float gameTime)
        {
            foreach (GUIComponent component in components)
            {
                component.Update(gameTime);
            }
        }

        /// <summary>
        /// Draws all the components in this page
        /// </summary>
        /// <param name="sb"></param>
        public void Draw(SpriteBatch sb)
        {
            foreach (GUIComponent component in components)
            {
                component.Draw(sb);
            }
        }

        /// <summary>
        /// Checks if a button was clicked
        /// </summary>
        /// <param name="identifier"></param>
        public bool Clicked(InputManager input, string identifier)
        {
            Button button = (Button)components.Find(btn => btn.Identifier == identifier);

            return button != null && button.Clicked(input);
        }

        public bool Hover(InputManager input, string identifier)
        {
            Button button = (Button)components.Find(btn => btn.Identifier == identifier);

            return button != null && button.Hover(input);
        }

        /// <summary>
        /// Shows the specified component
        /// </summary>
        /// <param name="identifier"></param>
        public void Show(string identifier)
        {
            GUIComponent component = components.Find(comp => comp.Identifier == identifier);

            // If the component is not null, show the component
            if (component != null)
            {
                component.Show();
            }
            else
            {
                throw new NullReferenceException(
                    "Attempted to show a component that does not exist. The component you tried to access was named \"" +
                    identifier + "\"");
            }
        }

        /// <summary>
        /// Shows all components with this page
        /// </summary>
        public void ShowAll()
        {
            foreach (GUIComponent component in components)
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
            GUIComponent component = components.Find(comp => comp.Identifier == identifier);

            // If the component is not null, show the component
            if (component != null)
            {
                component.Hide();
            }
            else
            {
                throw new NullReferenceException(
                    "Attempted to show a component that does not exist. The component you tried to access was named \"" +
                    identifier + "\"");
            }
        }

        /// <summary>
        /// Hides all components associated with this manager
        /// </summary>
        public void HideAll()
        {
            foreach (GUIComponent component in components)
            {
                component.Hide();
            }
        }
    }
}