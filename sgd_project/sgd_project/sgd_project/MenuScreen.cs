//File:     MenuScreen.cs
//Name:     Samuel Lewis (5821103) & Thomas Kempton (5781000)
//Date:     2013-03-19
//Class:    Simulation and Game Development
//Ass:      3
//
//Desc:     Describes a single menu screen

using System;
using System.Collections.Generic;

namespace sgd_project
{
    /// <summary>
    /// A single screen in the menu system
    /// </summary>
    internal class MenuScreen
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MenuScreen"/> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="parent">The parent.</param>
        public MenuScreen(String title, MenuScreen parent)
        {
            Title = title;
            Parent = parent;
        }

        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public String Title { get; private set; }

        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <value>
        /// The parent.
        /// </value>
        public MenuScreen Parent { get; private set; }

        /// <summary>
        /// Gets or sets the elements.
        /// </summary>
        /// <value>
        /// The elements.
        /// </value>
        public Dictionary<String, Action> Elements { get; set; }

        /// <summary>
        /// Gets or sets the index of the selected.
        /// </summary>
        /// <value>
        /// The index of the selected.
        /// </value>
        public int SelectedIndex { get; set; }
    }
}