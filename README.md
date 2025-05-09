# 🛠️ Unity CAD Tool (Midpoint Algorithm Based)

A basic **2D CAD (Computer-Aided Design)** application developed in **Unity**. This project focuses on implementing key computer graphics concepts like the **Midpoint algorithm** for drawing shapes and interactive manipulation of objects such as lines, circles, ellipses, and curves.

---

## ✨ Features

- Draw shapes using Midpoint-based algorithms
- Interactive shape selection and transformation
- **Shape List Panel** (press Tab to show/hide)
- Support for:
  - **Line**
  - **Circle**
  - **Ellipse**
  - **Hermite Curve**
  - **Bezier Curve**
- Keyboard-driven interface for mode switching and precision input
- JSON-based saving and loading of shapes with color and geometric data

---

## 🕹️ How to Use

### 🔄 Mode Switching

Press the following keys to switch between interaction modes:

| Key | Mode            | Description                                |
|-----|------------------|--------------------------------------------|
| `1` | **Select Mode**  | Click to select shapes. Press `R` to rotate, `F` to move. |
| `2` | **Line Mode**    | Begin drawing lines using the Midpoint algorithm. |
| `3` | **Circle Mode**  | Draw circles based on center and radius. |
| `4` | **Ellipse Mode** | Draw ellipses based on center and radii. |
| `5` | **Hermite Curve**| Draw Hermite curves using 4 control points. |
| `6` | **Bezier Curve** | Draw cubic Bezier curves using 4 control points. |
| `Tab` | **Shape List** | Toggle panel showing all created shapes for quick selection |

### 🗂️ Shape List Panel
- Press `Tab` to show/hide the shape list panel
- Click on any shape in the list to select it
- Selected shapes can be transformed (move/rotate) in Select Mode

### 🔢 Precision Input

While in any mode, press Right Mouse to precisely define control points or parameters (e.g., radius, coordinates).

---

## 💾 Save & Load

Press ctrl + s to save the shapes can be saved to and auto loaded when start from a `shapes.json` file in the system's persistent data path. The saved data includes:

- Shape type
- Position/control points
- Radius or radii (for circle/ellipse)
- Color (stored in hex format)
