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
