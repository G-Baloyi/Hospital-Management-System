using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PRG281_Project
{
    internal class Animations
    {
        //The Initial Load Animation
        public void AniIntro(int durationMs)
        {
            DateTime end = DateTime.UtcNow.AddMilliseconds(durationMs);
            bool originalCursor = Console.CursorVisible;
            Console.CursorVisible = false;

            // Characters reminiscent of Matrix glyphs
            string glyphs =
                "アイウエオカキクケコサシスセソタチツテトナニヌネノ" +
                "ハヒフヘホマミムメモヤユヨラリルレロワン0123456789" +
                "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            var rando = new Random();

            int width = Math.Max(40, Console.WindowWidth);
            int height = Math.Max(12, Console.WindowHeight);

            int step = 2; // use every 2 columns to reduce flicker
            int cols = width / step;

            // y positions and speeds for each active column
            int[] y = new int[cols];
            int[] speed = new int[cols];

            for (int i = 0; i < cols; i++)
            {
                // start above the screen at random offsets
                y[i] = -rando.Next(0, height);
                speed[i] = 1 + rando.Next(0, 2); // 1..2
            }

            try
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Clear();

                while (DateTime.UtcNow < end)
                {
                    // Allow user to skip animation
                    if (Console.KeyAvailable)
                    {
                        // consume the key so menu doesn't immediately act on it
                        Console.ReadKey(true);
                        break;
                    }

                    width = Math.Max(40, Console.WindowWidth);
                    height = Math.Max(12, Console.WindowHeight);
                    cols = width / step;

                    // Draw/advance columns
                    for (int i = 0; i < cols && i < y.Length; i++)
                    {
                        int x = i * step;

                        // Advance
                        y[i] += speed[i];
                        if (y[i] - 6 > height + 10)
                        {
                            y[i] = -rando.Next(0, height);
                            speed[i] = 1 + rando.Next(0, 2);
                        }

                        // Head position and trail positions
                        int head = y[i];
                        int t1 = head - 1;
                        int t2 = head - 2;
                        int t3 = head - 3;
                        int clearPos = head - 6;

                        // Head (bright)
                        if (head >= 0 && head < height) SafeDraw(x, head, RandomSymbol(glyphs, rando), ConsoleColor.White, ConsoleColor.Black);

                        // Trail (various greens)
                        if (t1 >= 0 && t1 < height) SafeDraw(x, t1, RandomSymbol(glyphs, rando), ConsoleColor.Green, ConsoleColor.Black);
                        if (t2 >= 0 && t2 < height) SafeDraw(x, t2, RandomSymbol(glyphs, rando), ConsoleColor.DarkGreen, ConsoleColor.Black);
                        if (t3 >= 0 && t3 < height) SafeDraw(x, t3, RandomSymbol(glyphs, rando), ConsoleColor.DarkGreen, ConsoleColor.Black);

                        // Clear far tail
                        if (clearPos >= 0 && clearPos < height)
                        {
                            try
                            {
                                Console.SetCursorPosition(x, clearPos);
                                Console.Write(' ');
                            }
                            catch { /* ignore tiny terminals */ }
                        }
                    }

                    Thread.Sleep(35);
                }
            }
            finally
            {
                Console.ResetColor();
                Console.Clear();
                Console.CursorVisible = originalCursor;
            }
        }

        //Generates random symbols for the Initial Load Animation
        public char RandomSymbol(string symbol, Random random)
        {
            return symbol[random.Next(0, symbol.Length)];
        }

        //Used to draw one symbol at an exact x, y (Row, Column) position
        public void SafeDraw(int inRow, int inCol, char inChar, ConsoleColor foreground, ConsoleColor background)
        {
            try    //Also prevents the animation from crashing when resizing the console app.
            {
                Console.SetCursorPosition(inRow, inCol);
                Console.ForegroundColor = foreground;
                Console.BackgroundColor = background;
                Console.Write(inChar);
            }
            catch
            {
            }
        }

        //Animation that welcomes you to the project after the Intro Animation
        public void WelcomeProjectAni(int durationMs)
        {
            string msg = "WELCOME TO THE PROJECT";
            int delayMs = 40;
            int frames = Math.Max(1, durationMs / delayMs);

            bool originalCursor = Console.CursorVisible;
            Console.CursorVisible = false;

            try
            {
                for (int f = 0; f < frames; f++)
                {
                    if (Console.KeyAvailable)
                    {
                        Console.ReadKey(true);
                        break;
                    }

                    Console.Clear();
                    int width = Math.Max(Console.WindowWidth, 40);
                    int height = Math.Max(Console.WindowHeight, 12);

                    int row = height / 2;

                    // Draw a subtle top hint
                    string subtitle = "Loading…";
                    int subPad = Math.Max(0, (width - subtitle.Length) / 2);
                    if (subtitle.Length + subPad > width) subPad = Math.Max(0, width - subtitle.Length);
                    Console.SetCursorPosition(0, Math.Max(0, row - 2));
                    if (subPad > 0) Console.Write(new string(' ', subPad));
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine(subtitle);

                    // Draw the animated title
                    int pad = Math.Max(0, (width - msg.Length) / 2);
                    if (msg.Length + pad > width) pad = Math.Max(0, width - msg.Length);

                    Console.SetCursorPosition(0, row);
                    // left padding uncolored
                    if (pad > 0) Console.Write(new string(' ', pad));

                    for (int i = 0; i < msg.Length; i++)
                    {
                        // Wave function across characters, moving over time
                        double t = (f * 0.25) + (i * 0.6);
                        double s = (Math.Sin(t) + 1.0) * 0.5; // 0..1

                        ConsoleColor color;
                        if (s > 0.80) color = ConsoleColor.White;
                        else if (s > 0.60) color = ConsoleColor.Cyan;
                        else if (s > 0.40) color = ConsoleColor.Green;
                        else color = ConsoleColor.DarkGreen;

                        Console.ForegroundColor = color;
                        Console.Write(msg[i]);
                    }

                    // underline (soft)
                    string underline = new string('─', msg.Length);
                    int uPad = pad;
                    Console.SetCursorPosition(0, Math.Min(row + 1, height - 1));
                    if (uPad > 0) Console.Write(new string(' ', uPad));
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write(underline);

                    Console.ResetColor();
                    Thread.Sleep(delayMs);
                }
            }
            finally
            {
                Console.ResetColor();
                Console.Clear();
                Console.CursorVisible = originalCursor;
            }
        }

        //Animation that is displayed if you successfully logged in
        public void LogInSuccessAni(string name)
        {
            string msg = "Welcome, " + name + "!";
            var colors = new ConsoleColor[]
            {
                ConsoleColor.Cyan, ConsoleColor.Blue, ConsoleColor.Magenta,
                ConsoleColor.Yellow, ConsoleColor.Green, ConsoleColor.White
            };

            int frames = 36;
            int delayMs = 45;

            var rnd = new Random();

            for (int f = 0; f < frames; f++)
            {
                Console.Clear();

                int width = Math.Max(Console.WindowWidth, 40);
                int height = Math.Max(Console.WindowHeight, 12);

                int row = height / 2;
                int pad = Math.Max(0, (width - msg.Length) / 2);
                if (msg.Length + pad > width) pad = Math.Max(0, width - msg.Length);

                // Shadow/glow under the text
                Console.SetCursorPosition(0, Math.Min(row + 1, height - 1));
                if (pad > 0) Console.Write(new string(' ', pad));
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine(msg);

                // Main neon text with cycling color
                Console.SetCursorPosition(0, row);
                if (pad > 0) Console.Write(new string(' ', pad));
                Console.ForegroundColor = colors[f % colors.Length];
                Console.WriteLine(msg);

                // Sparkles/confetti around the text line
                int sparkles = 24;
                for (int i = 0; i < sparkles; i++)
                {
                    int sRow = row + (rnd.Next(0, 3) - 1); // around the message row
                    int sCol = Math.Max(0, pad + rnd.Next(-6, msg.Length + 6));
                    try
                    {
                        Console.SetCursorPosition(sCol, Math.Max(0, Math.Min(height - 1, sRow)));
                        Console.ForegroundColor = colors[rnd.Next(colors.Length)];
                        Console.Write(rnd.Next(0, 3) == 0 ? '★' : '•');
                    }
                    catch { }
                }

                Console.ResetColor();
                Thread.Sleep(delayMs);
            }

            Console.ResetColor();
            Console.WriteLine();
        }

        //Animation that simulates an old typewriter typing when creating a new account
        public void TypeAni(string outText, ConsoleColor color)
        {
            Console.WriteLine();
            Console.ForegroundColor = color;
            Console.Write("> ");
            for (int i = 0; i < outText.Length; i++)
            {
                Console.Write(outText[i]);
                Thread.Sleep(25);
            }
            Console.WriteLine();
            Console.ResetColor();
        }

        //Animation that simulates loading when adding a new account to the textfile
        public void Spinner(int inTicks, ConsoleColor color)
        {
            char[] sequence = new[] { '|', '/', '-', '\\' };
            Console.ForegroundColor = color;
            Console.Write("   ");
            for (int i = 0; i < inTicks; i++)
            {
                Console.Write("\b\b\b" + " " + sequence[i % sequence.Length] + " ");
                Thread.Sleep(80);
            }
            Console.WriteLine();
            Console.ResetColor();
        }
    }
}