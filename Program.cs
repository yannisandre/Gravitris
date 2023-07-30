using System;
using Raylib_CsLo;
using System.Numerics;
using System.Collections;


public class Program
{
    public class Bricks
    {
        private int line;
        private int column;

        public Bricks(int line, int column)
        {
            this.line = line;
            this.column = column;
        }

        public int Line { get { return line; } set { line = value; } }
        public int Column { get { return column; } set { column = value; } }

    }
    public class Shape
    {
        private List<List<(int, int)>> list_rotations;
        private List<Bricks> list_bricks;
        private List<Bricks> list_bricks_backup = new List<Bricks>();
        private int color_number;
        private Texture color;
        private Texture empty_brick;
        private bool fell = false;
        private int rotation_index = 0;
        private int brick_size;
        private bool initial_condition = true;
        private float score = 0;

        public Shape(List<List<(int, int)>> list_rotations, List<Bricks> list_bricks, Texture color, Texture empty_brick, int color_number, int brick_size)
        {
            this.list_rotations = list_rotations;
            this.list_bricks = list_bricks;
            this.color = color;
            this.color_number = color_number;
            this.brick_size = brick_size;
            this.empty_brick = empty_brick;
            foreach (Bricks bricks in list_bricks) { list_bricks_backup.Add(bricks); }

            this.brick_size = brick_size;
        }

        public void display()
        {
            foreach (Bricks bricks in list_bricks)
            {
                if (bricks.Line >= 4)
                {
                    Raylib.DrawTexture(color, bricks.Column * brick_size + 120, (bricks.Line - 4) * brick_size + 130, Raylib.WHITE);
                }

            }
        }

        public int[][] change_board(int[][] board)
        {
            foreach (Bricks bricks in list_bricks)
            {
                board[bricks.Line][bricks.Column] = color_number;
            }
            return board;

        }

        public int[][] falling(int[][] board)
        {
            
            // selection sort -> sorting bricks depending on their line number in descending order
            List<Bricks> list_bricks_backupe = list_bricks;
            Bricks[] list_bricks_descending = new Bricks[list_bricks.Count];
            int index = 0;
            for (int i = 0; i < list_bricks.Count; i++)
            {
                int index_brick_max = i;
                for (int j = i; j < list_bricks.Count; j++)
                {
                    if (list_bricks[j].Line > list_bricks[index_brick_max].Line)
                    {
                        index_brick_max = j;
                    }
                }
                Bricks temp = list_bricks[i];
                list_bricks[i] = list_bricks[index_brick_max];
                list_bricks[index_brick_max] = temp;
                list_bricks_descending[index] = list_bricks[i];
                index++;
            }
            list_bricks = list_bricks_backupe;
            
            
            
            bool test = true;
            bool test2 = false;
            List<(int, int)> liste_coords_bricks = new List<(int, int)>();
            foreach (Bricks bricks in list_bricks_descending)
            {
                test2 = false;
                foreach ((int,int) b in liste_coords_bricks) { if (b == (bricks.Line+1,bricks.Column)) { test2 = true; } }
                if (bricks.Line + 1 >= board.Length || (board[bricks.Line + 1][bricks.Column] != 0 && !(test2)) ) 
                { 
                    test = false; 
                    this.fell = true;
                    if (bricks.Line + 1 < board.Length)
                    {
                        if ((board[bricks.Line + 1][bricks.Column] != 0 && !(test2)) && bricks.Line + 1 == 4)
                        {
                            Console.WriteLine("game over");
                            initial_condition = false;
                        }
                    }
                    
                    break; 
                }
                liste_coords_bricks.Add((bricks.Line, bricks.Column));
            }

           

            if (test)
            {
                this.erase();
                foreach (Bricks bricks in list_bricks)
                {
                    board[bricks.Line][bricks.Column] = 0;
                    bricks.Line += 1;
                    board[bricks.Line][bricks.Column] = color_number;
                    if (bricks.Line >= 4)
                    {
                        Raylib.DrawTexture(color, bricks.Column * brick_size + 120, (bricks.Line - 4) * brick_size + 130, Raylib.WHITE);
                    }

                }
            }
            return board;
            
        }

        public int[][] mooving(int[][] board, int delta)
        {
            // selection sort -> sorting bricks by their column number in descending order
            List<Bricks> list_bricks_backupe = list_bricks;
            Bricks[] list_bricks_descending = new Bricks[list_bricks.Count];
            int index = 0;
            if (delta > 0)
            {
                for (int i = 0; i < list_bricks.Count; i++)
                {
                    int index_brick_max = i;
                    for (int j = i; j < list_bricks.Count; j++)
                    {
                        if (list_bricks[j].Column > list_bricks[index_brick_max].Column)
                        {
                            index_brick_max = j;
                        }
                    }
                    Bricks temp = list_bricks[i];
                    list_bricks[i] = list_bricks[index_brick_max];
                    list_bricks[index_brick_max] = temp;
                    list_bricks_descending[index] = list_bricks[i];
                    index++;
                }
            }
            else
            {
                for (int i = 0; i < list_bricks.Count; i++)
                {
                    int index_brick_max = i;
                    for (int j = i; j < list_bricks.Count; j++)
                    {
                        if (list_bricks[j].Column < list_bricks[index_brick_max].Column)
                        {
                            index_brick_max = j;
                        }
                    }
                    Bricks temp = list_bricks[i];
                    list_bricks[i] = list_bricks[index_brick_max];
                    list_bricks[index_brick_max] = temp;
                    list_bricks_descending[index] = list_bricks[i];
                    index++;
                }
            }

            list_bricks = list_bricks_backupe;

            bool test = true;
            bool test2 = false;
            List<(int, int)> liste_coords_bricks = new List<(int, int)>();
            foreach (Bricks bricks in list_bricks_descending)
            {
                test2 = false;
                foreach ((int, int) b in liste_coords_bricks) { if (b == (bricks.Line, bricks.Column + delta)) { test2 = true; } }
                if (bricks.Column + delta >= board[0].Length || bricks.Column + delta < 0 || (board[bricks.Line][bricks.Column + delta] != 0 && !(test2)))
                {
                    test = false;
                    break;
                }
                liste_coords_bricks.Add((bricks.Line, bricks.Column));
            }

            if (test)
            {
               
                this.erase();
                foreach (Bricks bricks in list_bricks)
                {
                    board[bricks.Line][bricks.Column] = 0;
                    bricks.Column += delta;
                    board[bricks.Line][bricks.Column] = color_number;
                    if (bricks.Line >= 4)
                    {
                        Raylib.DrawTexture(color, bricks.Column * brick_size + 120, (bricks.Line - 4) * brick_size + 130, Raylib.WHITE);
                    }

                }
            }
            return board;



        }

        public void erase()
        {
            foreach (Bricks bricks in list_bricks)
            {
                if (bricks.Line >= 4)
                {
                    Raylib.DrawTexture(empty_brick, bricks.Column * brick_size + 120, (bricks.Line - 4) * brick_size + 130, Raylib.WHITE);
                }

            }
        }

        public int[][] rotation(int[][] board)
        {

            List<(int, int)> liste_coords_bricks = new List<(int, int)>();
            foreach (Bricks bricks in list_bricks)
            {
                liste_coords_bricks.Add((bricks.Line, bricks.Column));
            }
            int index_bricks = 0;
            bool teste = true;
            foreach (Bricks bricks in list_bricks)
            {
                teste = true;
                int line_rotation = list_rotations[rotation_index][index_bricks].Item1;
                int column_rotation = list_rotations[rotation_index][index_bricks].Item2;

                index_bricks++;
                foreach ( (int,int) b in liste_coords_bricks)
                {
                    if (b == (bricks.Line + line_rotation, bricks.Column + column_rotation))
                    {
                        teste = false;
                    }
                }
                if ( bricks.Line + line_rotation < 0 || bricks.Line + line_rotation >= board.Length || bricks.Column + column_rotation < 0 || bricks.Column + column_rotation >= board[0].Length)
                {
                    return board;
                }
                if (board[bricks.Line + line_rotation][bricks.Column + column_rotation] != 0 && teste)
                {
                    return board;
                }
            }
            
            index_bricks = 0;
            this.erase();
            List<Bricks> backupes = new List<Bricks>();
            foreach( Bricks bricks in list_bricks_backup) { board[bricks.Line][bricks.Column] = 0; backupes.Add(bricks); }
            list_bricks = backupes;

            foreach (Bricks bricks in list_bricks)
            {
                int line_rotation = list_rotations[rotation_index][index_bricks].Item1;
                int column_rotation = list_rotations[rotation_index][index_bricks].Item2;

                bricks.Line += line_rotation;
                bricks.Column += column_rotation;
                board[bricks.Line][bricks.Column] = color_number;
                if (bricks.Line >= 4)
                {
                    Raylib.DrawTexture(color, bricks.Column * brick_size + 120, (bricks.Line - 4) * brick_size + 130, Raylib.WHITE);
                }
                index_bricks++;

            }

            if (rotation_index + 1 == list_rotations.Count)
            {
                rotation_index = 0;
            }

            else
            {
                rotation_index+= 1;
            }
            return board;

        }


        public bool test_lines_vanishing(int[][] board)
        {
            for (int i = board.Length - 1; i >= 4; i--)
            {
                bool test_full = true;
                for (int j = 0; j < board[i].Length; j++)
                {
                    if (board[i][j] == 0) { test_full = false; break; }
                }
                if (test_full)
                {
                    return true;
                }
            }
            return false;
        }



        public int[][] lines_vanishing(int[][] board)
        {
            for (int i = board.Length - 1; i >= 4; i--)
            {
                bool test_full = true;
                for (int j = 0; j < board[i].Length; j++)
                {
                    if (board[i][j] == 0) { test_full = false; break; }
                }
                if (test_full)
                {
                    for (int j = 0; j < board[i].Length; j++)
                    {
                        board[i][j] = 0;
                    }
                    score += 1000;
                }
            }
            return board;
        }

        public int[][] check_lines(int[][] board)
        {
            
            for (int i = board.Length-2; i >= 4; i--)
            {
                for (int o = 0; o < board[i].Length; o++)
                {
                    if (board[i + 1][o] == 0)
                    {
                        int fall_index = i + 1;
                        while (fall_index < board.Length && board[fall_index][o] == 0)
                        {
                            int temp = board[fall_index - 1][o];
                            board[fall_index - 1][o] = 0;
                            board[fall_index][o] = temp;
                            fall_index++;
                        }
                    }
                }
            }
            return board;
        }

        public void set_preview(int[][] board, Texture brick_preview)
        {
            int[][] temp_board = new int[board.Length][];
            for (int i = 0; i < board.Length; i++)
            {
                int[] temp_tab = new int[board[0].Length];
                for (int j = 0; j < board[0].Length; j++)
                {
                    temp_tab[j] = board[i][j];
                }
                temp_board[i] = temp_tab;
            }

            int max_line = list_bricks[0].Line;
            List<int[]> list_height_variations = new List<int[]>();
            int index_brick = 0;
            bool cond = true;
            foreach (Bricks bricks in list_bricks)
            {
                int[] temp_tab = { 0, bricks.Line, bricks.Column };
                list_height_variations.Add(temp_tab);
                temp_board[bricks.Line][bricks.Column] = 0;
                if (bricks.Line > max_line) { max_line = bricks.Line; }
            }
            while (cond && max_line < temp_board.Length - 1)
            {
  
                foreach(Bricks bricks in list_bricks)
                {
                  
                    if (temp_board[bricks.Line + list_height_variations[index_brick][0] + 1 ][bricks.Column] != 0) 
                    {
                      
                        cond = false; 
                        for (int i = 0; i < index_brick; i++)
                        {
                            list_height_variations[i][0]--;
                        }
                        break;
                        

                    }

                    list_height_variations[index_brick][0]++;
                    index_brick++;
                    

                }
                max_line++;
                index_brick = 0;
            }
            index_brick = 0;
            foreach(int[] couple in list_height_variations) { Raylib.DrawTexture(brick_preview, (couple[2]) * 20 + 120, (couple[0] + couple[1] - 4) * 20 + 130, Raylib.WHITE); }


        }


        public bool Fell { get { return fell; } }
        public bool Initial_condition { get { return initial_condition; } }
        public float Score { get { return score;  } }


    }






        static void GameOver(Texture game_over)
        {
            Raylib.DrawTexture(game_over, 0, 0, Raylib.WHITE);
        }
    
        static void display_score(string texte, float score, int x, int y, int size, Color color)
        {
            if (score >= 1000)
            {
                Raylib.DrawText(texte + " : " + "\n" + "  " + (int)score / 1000 + "K", x, y, size, color);
            }
            else if (score >= 1000000)
            {
                Raylib.DrawText(texte + " : " + "\n" + "  " + (int)score / 1000000 + "M", x, y, size, color);
            }
            else
            {
                Raylib.DrawText(texte + " : " + "\n" + "  " + (int)score, x, y, size, color);
            }
        }


        static void display_board(int[][] board, Texture empty_brick, Texture[] list_textures, int brick_color_number, List<(int, int)>[] list_bricks_coords, int random_list_shapes, Texture background)
        {
            // displays board
            Raylib.DrawTexture(background, 0, 0, Raylib.WHITE);
            Raylib.DrawText("GRAVITRIS", 275, 30, 50, Raylib.RED);
            int x = 120;
            int y = 130;
            Texture choice = empty_brick;
            for (int i = 4; i < board.Length; i++)
            {
                for (int j = 0; j < board[i].Length; j++)
                {
                    if (board[i][j] == 0) { Raylib.DrawTexture(empty_brick, x, y, Raylib.WHITE); }
                    else { Raylib.DrawTexture(list_textures[board[i][j]-1], x, y, Raylib.WHITE); }
                    x += 20;
                }
                x = 120;
                y += 20;
            }
            Raylib.DrawLine(390, 250, 390, 151, Raylib.YELLOW);
            Raylib.DrawLine(391, 250, 391, 150, Raylib.BLACK);
            Raylib.DrawLine(391, 150, 560, 150, Raylib.YELLOW);
            Raylib.DrawLine(390, 151, 560, 151, Raylib.BLACK);

            Raylib.DrawLine(560, 151, 560, 250, Raylib.YELLOW);
            Raylib.DrawLine(561, 151, 561, 250, Raylib.BLACK);

            Raylib.DrawLine(390, 250, 560, 250, Raylib.YELLOW);
            Raylib.DrawLine(390, 249, 559, 249, Raylib.BLACK);
            foreach( (int,int) coords in list_bricks_coords[random_list_shapes])
            {
                Raylib.DrawTexture(list_textures[brick_color_number], (coords.Item1 + 16) * 20 + 120, (coords.Item2 - 1 ) * 20 + 130, Raylib.WHITE);
            }
        }





        public static Task Main(string[] args)
        {

            Raylib.InitWindow(600, 820, "GRAVITRIS");
            Raylib.SetTargetFPS(60);

            Texture empty_brick = Raylib.LoadTexture("C:/Users/yanni/Desktop/fourre-tout A2/Simulation/bin/Debug/net6.0/Pictures/empty_brick.png");
            empty_brick.width = 20;
            empty_brick.height = 20;
            Texture preview_brick = Raylib.LoadTexture("C:/Users/yanni/Desktop/fourre-tout A2/Simulation/bin/Debug/net6.0/Pictures/preview_brick2.png");
            preview_brick.width = 20;
            preview_brick.height = 20;
            Texture red_brick = Raylib.LoadTexture("C:/Users/yanni/Desktop/fourre-tout A2/Simulation/bin/Debug/net6.0/Pictures/red_brick.png");
            Texture yellow_brick = Raylib.LoadTexture("C:/Users/yanni/Desktop/fourre-tout A2/Simulation/bin/Debug/net6.0/Pictures/yellow_brick.png");
            Texture blue_brick = Raylib.LoadTexture("C:/Users/yanni/Desktop/fourre-tout A2/Simulation/bin/Debug/net6.0/Pictures/blue_brick.png");
            Texture lblue_brick = Raylib.LoadTexture("C:/Users/yanni/Desktop/fourre-tout A2/Simulation/bin/Debug/net6.0/Pictures/lblue_brick.png");
            Texture purple_brick = Raylib.LoadTexture("C:/Users/yanni/Desktop/fourre-tout A2/Simulation/bin/Debug/net6.0/Pictures/purple_brick.png");
            Texture orange_brick = Raylib.LoadTexture("C:/Users/yanni/Desktop/fourre-tout A2/Simulation/bin/Debug/net6.0/Pictures/orange_brick.png");
            Texture background = Raylib.LoadTexture("C:/Users/yanni/Desktop/fourre-tout A2/Simulation/bin/Debug/net6.0/Pictures/background2.png");
            Texture game_over = Raylib.LoadTexture("C:/Users/yanni/Desktop/fourre-tout A2/Simulation/bin/Debug/net6.0/Pictures/game_over.png");
            background.width = 620;
            game_over.width = 620;
            background.height = 840;
            game_over.height = 840;

            Texture[] list_textures = new Texture[] { red_brick, yellow_brick, purple_brick, blue_brick, lblue_brick, orange_brick };
            for (int i = 0; i < list_textures.Length; i++)
            {
                list_textures[i].width = 20;
                list_textures[i].height = 20;
            }

            Random rand = new Random();
            int random_texture = rand.Next(list_textures.Length);

            // first shape "I"

            // first rotation (clockwise)
            List<(int, int)> list_temp1_I = new List<(int, int)>() { (1, 0), (0, 1), (-1, 2), (-2, 3) };

            // second rotation 
            List<(int, int)> list_temp2_I = new List<(int, int)>() { (-1, 0), (0, -1), (1, -2), (2, -3) };
            // list bricks
            List<(int, int)> list_coords_bricks_I = new List<(int, int)>() { (0, 4), (1, 4), (2, 4), (3,4) };
            Bricks brick;
            List<Bricks> list_bricks_I = new List<Bricks>();
            foreach ((int, int) bricks in list_coords_bricks_I)
            {
                brick = new Bricks(bricks.Item1, bricks.Item2);
                list_bricks_I.Add(brick);
            }
          
            List<List<(int, int)>> list_rotations_I = new List<List<(int, int)>> { list_temp1_I, list_temp2_I };

            Shape shape_I = new Shape(list_rotations_I, list_bricks_I, list_textures[random_texture], empty_brick, random_texture+1, 20);

            // second shape "square"
            // no rotations so zero variations
            List<(int, int)> list_temp1_square = new List<(int, int)>() { (0, 0), (0, 0), (0, 0), (0, 0) };
            // list bricks 
            List<(int, int)> list_coords_bricks_square = new List<(int, int)>() { (0, 4), (0, 5), (1, 4), (1, 5) };
            List<Bricks> list_bricks_square = new List<Bricks>();
            foreach ((int, int) bricks in list_coords_bricks_square)
            {
                brick = new Bricks(bricks.Item1, bricks.Item2);
                list_bricks_square.Add(brick);
            }

            List<List<(int, int)>> list_rotations_square = new List<List<(int, int)>> { list_temp1_square };


            // third shape "L"

            // first rotation (clockwise)
            List<(int, int)> list_temp1_L = new List<(int, int)>() { (1,1), (0, 0), (0,-1), (-1, -2) };

            // second rotation 
            List<(int, int)> list_temp2_L = new List<(int, int)>() { (1, -1), (0, 0), (-2, 0), (-1, 1) };

            // third rotation 
            List<(int, int)> list_temp3_L = new List<(int, int)>() { (-1, -1), (0, 0), (0, 2), (1, 1) };
            // fourth rotation 
            List<(int, int)> list_temp4_L = new List<(int, int)>() { (-1, 1), (0, 0), (2, -1), (1, 0) };
            // list bricks
            List<(int, int)> list_coords_bricks_L = new List<(int, int)>() { (0, 4), (1, 4), (2, 4), (2, 5) };
            List<Bricks> list_bricks_L = new List<Bricks>();
            foreach ((int, int) bricks in list_coords_bricks_L)
            {
                brick = new Bricks(bricks.Item1, bricks.Item2);
                list_bricks_L.Add(brick);
            }

            List<List<(int, int)>> list_rotations_L = new List<List<(int, int)>> { list_temp1_L, list_temp2_L, list_temp3_L, list_temp4_L };

            


            // fourth shape "⅃"
            // first rotation (clockwise)
            List<(int, int)> list_temp1_inversed_L = new List<(int, int)>() { (1, 1), (0, 0), (-1, -1), (-2, 0) };

            // second rotation 
            List<(int, int)> list_temp2_inversed_L = new List<(int, int)>() { (1, -1), (0, 0), (-1, 1), (0, 2) };

            // third rotation 
            List<(int, int)> list_temp3_inversed_L = new List<(int, int)>() { (-1, -1), (0, 0), (1, 1), (2, 0) };
            // fourth rotation 
            List<(int, int)> list_temp4_inversed_L = new List<(int, int)>() { (-1, 1), (0, 0), (1, -1), (0, -2) };
            // list bricks
            List<(int, int)> list_coords_bricks_inversed_L = new List<(int, int)>() { (0, 4), (1, 4), (2, 4), (2, 3) };
            List<Bricks> list_bricks_inversed_L = new List<Bricks>();
            foreach ((int, int) bricks in list_coords_bricks_inversed_L)
            {
                brick = new Bricks(bricks.Item1, bricks.Item2);
                list_bricks_inversed_L.Add(brick);
            }

            List<List<(int, int)>> list_rotations_inversed_L = new List<List<(int, int)>> { list_temp1_inversed_L, list_temp2_inversed_L, list_temp3_inversed_L, list_temp4_inversed_L };


            // fith shape "Z"
            // first rotation (clockwise)
            List<(int, int)> list_temp1_Z = new List<(int, int)>() { (1, 1), (0, 0), (1, -1), (0, -2) };

            // second rotation 
            List<(int, int)> list_temp2_Z = new List<(int, int)>() { (1, -1), (0, 0), (-1, -1), (-2, 0) };

            // third rotation 
            List<(int, int)> list_temp3_Z = new List<(int, int)>() { (-1, -1), (0, 0), (-1, 1), (0, 2) };
            // fourth rotation 
            List<(int, int)> list_temp4_Z = new List<(int, int)>() { (-1, 1), (0, 0), (1, 1), (2, 0) };
            // list bricks
            List<(int, int)> list_coords_bricks_Z = new List<(int, int)>() { (0, 4), (1, 4), (1, 5), (2, 5) };
            List<Bricks> list_bricks_Z = new List<Bricks>();
            foreach ((int, int) bricks in list_coords_bricks_inversed_L)
            {
                brick = new Bricks(bricks.Item1, bricks.Item2);
                list_bricks_inversed_L.Add(brick);
            }

            List<List<(int, int)>> list_rotations_Z = new List<List<(int, int)>> { list_temp1_Z, list_temp2_Z, list_temp3_Z, list_temp4_Z };

            // sixth shape inversed "Z"
            // first rotation (clockwise)
            List<(int, int)> list_temp1_inversed_Z = new List<(int, int)>() { (2, 0), (1, -1), (0, 0), (-1, -1) };

            // second rotation 
            List<(int, int)> list_temp2_inversed_Z = new List<(int, int)>() { (0, -2), (-1, -1), (0, 0), (-1, 1) };

            // third rotation 
            List<(int, int)> list_temp3_inversed_Z = new List<(int, int)>() { (-2, 0), (-1, 1), (0, 0), (1, 1) };
            // fourth rotation 
            List<(int, int)> list_temp4_inversed_Z = new List<(int, int)>() { (0, 2), (1, 1), (0, 0), (1, -1) };
            // list bricks
            List<(int, int)> list_coords_bricks_inversed_Z = new List<(int, int)>() { (0, 5), (1, 5), (1, 4), (2, 4) };
            List<Bricks> list_bricks_inversed_Z = new List<Bricks>();
            foreach ((int, int) bricks in list_coords_bricks_inversed_Z)
            {
                brick = new Bricks(bricks.Item1, bricks.Item2);
                list_bricks_inversed_Z.Add(brick);
            }



            List<List<(int, int)>> list_rotations_inversed_Z = new List<List<(int, int)>> { list_temp1_inversed_Z, list_temp2_inversed_Z, list_temp3_inversed_Z, list_temp4_inversed_Z };

            // seventh shape "T"
            // first rotation (clockwise)
            List<(int, int)> list_temp1_T = new List<(int, int)>() { (1, 1), (0, 0), (1, -1), (-1, -1) };

            // second rotation 
            List<(int, int)> list_temp2_T = new List<(int, int)>() { (1, -1), (0, 0), (-1, -1), (-1, 1) };

            // third rotation 
            List<(int, int)> list_temp3_T = new List<(int, int)>() { (-1, -1), (0, 0), (-1, 1), (1, 1) };
            // fourth rotation 
            List<(int, int)> list_temp4_T = new List<(int, int)>() { (-1, 1), (0, 0), (1, 1), (1, -1) };
            // list bricks
            List<(int, int)> list_coords_bricks_T = new List<(int, int)>() { (0, 4), (1, 4), (1, 5), (2, 4) };
            List<Bricks> list_bricks_T = new List<Bricks>();
            foreach ((int, int) bricks in list_coords_bricks_T)
            {
                brick = new Bricks(bricks.Item1, bricks.Item2);
                list_bricks_T.Add(brick);
            }



            List<List<(int, int)>> list_rotations_T = new List<List<(int, int)>> { list_temp1_T, list_temp2_T, list_temp3_T, list_temp4_T };


            List<List<(int, int)>>[] list_shapes_rotations = { list_rotations_I, list_rotations_square, list_rotations_L, list_rotations_inversed_L, list_rotations_Z, list_rotations_inversed_Z, list_rotations_T};
            List<(int, int)>[] list_bricks_coords = { list_coords_bricks_I, list_coords_bricks_square, list_coords_bricks_L, list_coords_bricks_inversed_L, list_coords_bricks_Z, list_coords_bricks_inversed_Z, list_coords_bricks_T };

            // game board

            int[][] board = new int[34][];
            int[] ligne = new int[10];
            for (int i = 0; i < board.Length; i++)
            {
                ligne = new int[12];
                board[i] = ligne;
            }





            float score = 0;
            float high_score = 0;
            int timer = 0;
            int timer2 = 0;
            int timer3 = 0;
            bool initial_condition = true;
            bool flag = true;
            bool flag2 = true;
            int random_list_shapes = rand.Next(list_bricks_coords.Length);
            int next_random_list_shapes = rand.Next(list_bricks_coords.Length);
            while (next_random_list_shapes == random_list_shapes)
            {
                next_random_list_shapes = rand.Next(list_bricks_coords.Length);
            }
            int test_random = next_random_list_shapes;
            int next_random_texture = rand.Next(list_textures.Length);
            




            display_board(board, empty_brick, list_textures, next_random_texture, list_bricks_coords, next_random_list_shapes, background); 
            
            Raylib.SetTargetFPS(60);

            List<Bricks> list_brickse = new List<Bricks>();
            foreach ((int, int) bricks in list_bricks_coords[random_list_shapes])
            {
                brick = new Bricks(bricks.Item1, bricks.Item2);
                list_brickse.Add(brick);
            }   

            Shape shape = new Shape(list_shapes_rotations[random_list_shapes], list_brickse, list_textures[random_texture], empty_brick, random_texture + 1, 20);
            
            while (!(Raylib.WindowShouldClose()))
            {
                
                while (initial_condition)
                {
                    if (Raylib.WindowShouldClose()) { Raylib.CloseWindow(); Environment.Exit(0); }


                    Raylib.BeginDrawing();

                    Raylib.ClearBackground(Raylib.WHITE);

                    



                    timer3++;


                    if (Raylib.IsKeyDown(KeyboardKey.KEY_DOWN) && timer2 >= 1000)
                    {
                        timer += 10;
                        score += 0.001f;
                        if (score > high_score) { high_score = score; }

                    }
                    timer2++;
                    display_board(board, empty_brick, list_textures, next_random_texture, list_bricks_coords, next_random_list_shapes, background);
                    shape.set_preview(board, preview_brick);
                    display_score("Score",score, 380, 350, 50, Raylib.YELLOW);
                    display_score("High Score",high_score, 380, 560, 20, Raylib.RED);


                    timer++;
                    if (flag || timer >= 2000)
                    {

                        if (Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT))
                        {
                            board = shape.mooving(board, 1);

                        }
                        if (Raylib.IsKeyDown(KeyboardKey.KEY_LEFT))
                        {
                            board = shape.mooving(board, -1);
                        }

                        if (Raylib.IsKeyPressed(KeyboardKey.KEY_UP))
                        {
                            board = shape.rotation(board);
                        }
                        flag = false;

                        if (flag2) { shape.display(); board = shape.change_board(board); flag2 = false; }

                        board = shape.falling(board);


                        timer = 0;
                        if (shape.Fell)
                        {
                            initial_condition = shape.Initial_condition;
                            while (shape.test_lines_vanishing(board))
                            {
                                board = shape.lines_vanishing(board);
                                board = shape.check_lines(board);
                                score += shape.Score;
                                if (score > high_score) { high_score = score; }
                            }

                            int temp = next_random_list_shapes;
                            next_random_list_shapes = rand.Next(list_bricks_coords.Length);
                            while (next_random_list_shapes == test_random)
                            {
                                next_random_list_shapes = rand.Next(list_bricks_coords.Length);
                            }
                            random_list_shapes = temp;
                            test_random = next_random_list_shapes;
                            List<(int, int)> list_coords_bricks = list_bricks_coords[random_list_shapes];
                            List<List<(int, int)>> list_rotations_shape = list_shapes_rotations[random_list_shapes];
                            List<Bricks> list_brickses = new List<Bricks>();
                            foreach ((int, int) bricks in list_coords_bricks)
                            {
                                brick = new Bricks(bricks.Item1, bricks.Item2);
                                list_brickses.Add(brick);
                            }
                            temp = next_random_texture;
                            next_random_texture = rand.Next(list_textures.Length);
                            random_texture = temp;
                            shape = new Shape(list_rotations_shape, list_brickses, list_textures[random_texture], empty_brick, random_texture + 1, 20);
                            flag = true;
                            flag2 = true;

                        }
                        Raylib.EndDrawing();

                    }



                }
                Raylib.BeginDrawing();
                GameOver(game_over);
                Raylib.EndDrawing();
                if (score > high_score)
                {
                    high_score = score;
                }

                if (Raylib.IsKeyDown(KeyboardKey.KEY_KP_ENTER) || Raylib.IsKeyDown(KeyboardKey.KEY_ENTER))
                {
                    initial_condition = true;
                    board = new int[34][];
                    ligne = new int[10];
                    for (int i = 0; i < board.Length; i++)
                    {
                        ligne = new int[12];
                        board[i] = ligne;
                    }
                    score = 0;

                }










        }

        Raylib.CloseWindow();

        return Task.CompletedTask;

        }
        
    }


