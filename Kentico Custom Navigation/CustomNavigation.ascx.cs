using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.PortalControls;
using System;
using System.Collections.Generic;

namespace CMSApp.CMSWebParts.Custom
{
    public partial class CustomNavigation : CMSAbstractWebPart
    {
        #region Properties

        public string NavigationType
        {
            get
            {
                return DataHelper.GetNotEmpty(GetValue("MenuType"), null);
            }
            set
            {
                SetValue("MenuType", value);
            }
        }

        private const string _categoryName = "CategoryName";
        private const string _siteName = "MySite";

        #endregion

        #region Protected Control Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            GetNavigationData();
        }
------------------------------

import pandas as pd
import matplotlib.pyplot as plt

# Load data from the Excel file
file_path = r"C:\Users\abirs\Downloads\Stakeholder_Map.xlsx"
df = pd.read_excel(file_path)

# Create a figure and axis
plt.figure(figsize=(10, 6))

# Plot the data points as a scatter plot
plt.scatter(df["Interest"], df["Influence"], color="blue", alpha=0.7)

# Add labels for each point
for i, row in df.iterrows():
    plt.text(row["Interest"] + 0.1, row["Influence"] + 0.1, row["Stakeholder"], fontsize=8)

# Draw quadrant lines
plt.axhline(y=df["Influence"].median(), color='r', linestyle='--', label="Median Influence")
plt.axvline(x=df["Interest"].median(), color='g', linestyle='--', label="Median Interest")

# Add axis labels and title
plt.xlabel("Level of Interest")
plt.ylabel("Level of Influence")
plt.title("Stakeholder Map with Quadrants")

# Show legend
plt.legend()

# Show the grid
plt.grid(alpha=0.3)

# Display the plot
plt.show()

        #endregion

        #region Custom Navigation Helper class and methods

        /// <summary>
        /// Custom navigation class
        /// </summary>
        internal class MySiteNavigation
        {
            public string PageName { get; set; }
            public string PageUrl { get; set; }
            public string CategoryName { get; set; }
        }

        /// <summary>
        /// Get Page Nodes depend upon category
        /// </summary>
        Func<string, List<MySiteNavigation>> GetFilteredSiteNavigation = (category) =>
        {
            List<MySiteNavigation> siteNavigation = new List<MySiteNavigation>();
            TreeProvider treeProvider = new TreeProvider();
            NodeSelectionParameters nodeSelectionParams = new NodeSelectionParameters() { SiteName = _siteName };
            InfoDataSet<TreeNode> pages = treeProvider.SelectNodes(nodeSelectionParams);
            foreach (TreeNode node in pages)
            {
                if (node.IsPublished)
                {
                    if (node.Categories.DisplayNames.Count > 0)
                    {
                        List<string> categoryList = new List<string>();
                        foreach (BaseInfo items in node.Categories.DisplayNames.Collection)
                        {
                            categoryList.Add(items.GetOriginalValue(_categoryName).ToString().ToLower());
                        }
                        if (string.IsNullOrEmpty(category))
                        {
                            siteNavigation.Add(new MySiteNavigation
                            {
                                PageName = node.NodeName,
                                PageUrl = node.AbsoluteURL,
                                CategoryName = categoryList.Count > 0 ? categoryList.ToArray()[0] : string.Empty
                            });
                        }
                        else
                        {
                            if (categoryList.Contains(category))
                            {
                                siteNavigation.Add(new MySiteNavigation
                                {
                                    PageName = node.NodeName,
                                    PageUrl = node.AbsoluteURL,
                                    CategoryName = categoryList.Count > 0 ? categoryList.ToArray()[0] : string.Empty
                                });
                            }
                        }
                    }
                }
            }
            return siteNavigation;
        };

        /// <summary>
        /// Load navigation data into Repeater Control
        /// </summary>
        private void GetNavigationData()
        {
            repeaterNavigation.DataSource = GetFilteredSiteNavigation(NavigationType);
            repeaterNavigation.DataBind();
        }
        #endregion

    }
}
