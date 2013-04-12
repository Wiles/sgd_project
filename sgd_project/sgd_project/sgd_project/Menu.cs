//File:     Menu.cs
//Name:     Samuel Lewis (5821103) & Thomas Kempton (5781000)
//Date:     2013-03-19
//Class:    Simulation and Game Development
//Ass:      3
//
//Desc:     Entity used to draw the menu

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace sgd_project
{
    /// <summary>
    /// Draws the menu and handles navigation
    /// </summary>
    internal class Menu
    {
        private bool _down;
        private bool _enter;
        private bool _escape;
        private SpriteFont _font;
        private SoundEffect _menuBack;

        private SoundEffect _menuMove;
        private SoundEffect _menuSelect;
        private bool _up;
        private Viewport _viewport;

        /// <summary>
        /// Initializes a new instance of the <see cref="Menu"/> class.
        /// </summary>
        public Menu()
        {
            Screens = new List<MenuScreen>();
        }

        /// <summary>
        /// Gets or sets the selected menu screen.
        /// </summary>
        /// <value>
        /// The selected menu screen.
        /// </value>
        public int SelectedMenuScreen { get; set; }

        /// <summary>
        /// Gets or sets the index of the main menu.
        /// </summary>
        /// <value>
        /// The index of the main menu.
        /// </value>
        public int MainMenuIndex { get; set; }

        /// <summary>
        /// Gets the screens.
        /// </summary>
        /// <value>
        /// The screens.
        /// </value>
        public List<MenuScreen> Screens { get; private set; }

        /// <summary>
        /// Draws the Entity.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            MenuScreen screen = Screens[SelectedMenuScreen];
            Vector2 textSize = _font.MeasureString(screen.Title);
            spriteBatch.DrawString(_font,
                                   screen.Title,
                                   new Vector2((_viewport.Width/2) + 2, textSize.Y + 2),
                                   Color.White, 0.0f,
                                   new Vector2(textSize.X/2, textSize.Y/2),
                                   1.0f, SpriteEffects.None, 0.0f);
            foreach (int i in Enumerable.Range(0, screen.Elements.Count))
            {
                string ele = screen.Elements.Keys.ToArray()[i];
                string text = (i == screen.SelectedIndex) ? string.Format("> {0} <", ele) : ele;

                Vector2 eleSize = _font.MeasureString(text);
                spriteBatch.DrawString(_font,
                                       text,
                                       new Vector2((_viewport.Width/2) + 2, (int) (textSize.Y*(i + 2.5))),
                                       Color.White, 0.0f,
                                       new Vector2(eleSize.X/2, textSize.Y/2),
                                       1.0f, SpriteEffects.None, 0.0f);
            }

            if (screen != Screens[MainMenuIndex])
            {
                string text = (screen.SelectedIndex == screen.Elements.Count)
                                  ? string.Format("> {0} <", "Back")
                                  : "Back";
                Vector2 eleSize = _font.MeasureString(text);
                spriteBatch.DrawString(_font,
                                       text,
                                       new Vector2((_viewport.Width/2) + 2, textSize.Y*(screen.Elements.Count + 3)),
                                       Color.White, 0.0f,
                                       new Vector2(eleSize.X/2, textSize.Y/2),
                                       1.0f, SpriteEffects.None, 0.0f);
            }
        }

        /// <summary>
        /// Updates the Entity.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="input">The input.</param>
        /// <param name="delta">The delta.</param>
        public void Update(GraphicsDevice graphics, Input input, long delta)
        {
            MenuScreen screen = Screens[SelectedMenuScreen];
            int max = (Screens[SelectedMenuScreen] == Screens[MainMenuIndex])
                          ? screen.Elements.Count
                          : screen.Elements.Count + 1;
            if (input.Down())
            {
                if (_down == false)
                {
                    _down = true;
                    _menuMove.Play();
                    screen.SelectedIndex += 1;
                    if (screen.SelectedIndex >= max)
                    {
                        screen.SelectedIndex = 0;
                    }
                }
            }
            else
            {
                _down = false;
            }
            if (input.Up())
            {
                if (_up == false)
                {
                    _up = true;
                    _menuMove.Play();
                    screen.SelectedIndex -= 1;
                    if (screen.SelectedIndex < 0)
                    {
                        screen.SelectedIndex = max - 1;
                    }
                }
            }
            else
            {
                _up = false;
            }
            if (input.Select())
            {
                if (_enter == false)
                {
                    _enter = true;
                    Action[] actions = screen.Elements.Values.ToArray();
                    if (screen.SelectedIndex == actions.Count())
                    {
                        _menuBack.Play();
                        SelectedMenuScreen = screen.Parent == null ? MainMenuIndex : Screens.IndexOf(screen.Parent);
                    }
                    else
                    {
                        Action action = screen.Elements.Values.ToArray()[screen.SelectedIndex];
                        if (action != null)
                        {
                            _menuSelect.Play();
                            action();
                        }
                    }
                }
            }
            else
            {
                _enter = false;
            }

            if (input.Escape())
            {
                if (_escape == false)
                {
                    _escape = true;
                    if (screen.Parent != null)
                    {
                        _menuBack.Play();

                        SelectedMenuScreen = Screens.IndexOf(screen.Parent);
                    }
                    else if (screen != Screens[MainMenuIndex])
                    {
                        _menuBack.Play();
                        SelectedMenuScreen = MainMenuIndex;
                    }
                }
            }
            else
            {
                _escape = false;
            }
        }

        /// <summary>
        /// Initializes the specified viewport.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="font">The font.</param>
        /// <param name="move">The move.</param>
        /// <param name="select">The select.</param>
        /// <param name="back">The back.</param>
        public void Initialize(Viewport viewport, SpriteFont font, SoundEffect move, SoundEffect select,
                               SoundEffect back)
        {
            _viewport = viewport;
            _font = font;
            _menuMove = move;
            _menuSelect = select;
            _menuBack = back;
        }

        /// <summary>
        /// Adds the menu screen.
        /// </summary>
        /// <param name="screen">The screen.</param>
        public void AddMenuScreen(MenuScreen screen)
        {
            Screens.Add(screen);
        }
    }
}