using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZombieSurvivor.View
{
    public class PlayerView
    {
        private Model.Player playerModel;

        public PlayerView(Model.Player player)
        {
            playerModel = player;
        }

        // Draw the player on the screen
        public void Draw(Graphics graphics)
        {
            if (playerModel == null) return;

            // For now, draw a simple white rectangle
            Brush playerBrush = new SolidBrush(Color.White);
            graphics.FillRectangle(playerBrush,
                                 playerModel._x,
                                 playerModel._y,
                                 playerModel.Width,
                                 playerModel.Height);

            // Add a black border
            Pen borderPen = new Pen(Color.Black, 2);
            graphics.DrawRectangle(borderPen,
                                 playerModel._x,
                                 playerModel._y,
                                 playerModel.Width,
                                 playerModel.Height);

            playerBrush.Dispose();
            borderPen.Dispose();
        }

    }
}
