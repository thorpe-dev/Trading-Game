using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.Xml.Linq;


namespace Main_Game
{
    public partial class MapScreen : UserControl, IScreen
    {
        /*************************/
        /* Stuff to be passed in */
        /*************************/
        // The theme to be used
        int theme = 0;
        // Light level of dungeon
        Double light_level = 0.5;
        // Boolean to check if this player should generate the dungeon and if it should be sent
        int charid = 2;
        bool first_player = false;
        bool only_player = false;
        // Number of squares per row/column in the dungeon (+2 because of borders)
        int dungeon_dimensions = 15;
        // Number of normal enemy encounters to generate
        int dungeon_enemies = 6;
        // Number of items to generate
        int dungeon_items = 6;

        // The current instance id
        int instance;
        // Global variable for the number of rows displayed on screen
        int map_dimensions = 3;
        // Arrays to store the map and the events in each square
        int[][] map;
        int[][] events;
        int[][] players;
        // The number of blank squares in the dungeon
        int size;
        // The theme of the dungeon
        DungeonTheme dungeon_theme;
        // Player's current location
        int player_row;
        int player_col;
        // What is currently visible on the player's map
        int[][] current_view;
        // The minimap (created dynamically at run time)
        Image[][] minimap;
        // The player marker on the minimap
        Image img_player_mm;
        // The array of images for encounters
        Image[][] encounters;
        // A random number generator
        Random rnd = new Random();


        public MapScreen()
        {
            InitializeComponent();
            fill_tilesets();
            create_minimap();
            // Set up the 'visited' array
            this.events = new int[this.dungeon_dimensions][];
            for (int i = 0; i < dungeon_dimensions; i++)
            {
                this.events[i] = new int[dungeon_dimensions];
            }
            for (int i = 0; i < this.dungeon_dimensions; i++)
            {
                for (int j = 0; j < this.dungeon_dimensions; j++)
                {
                    events[i][j] = 0;
                }
            }
            // Set up the 'other players' array
            this.players = new int[this.dungeon_dimensions][];
            for (int i = 0; i < dungeon_dimensions; i++)
            {
                this.players[i] = new int[dungeon_dimensions];
            }
            // Initialise current_view and current_events
            this.current_view = new int[this.map_dimensions][];
            this.encounters = new Image[this.map_dimensions][];
            int y_offset = 66;
            for (int i = 0; i < this.map_dimensions; i++)
            {
                this.current_view[i] = new int[this.map_dimensions];
                this.encounters[i] = new Image[this.map_dimensions];
                int x_offset = 66;
                for (int j = 0; j < encounters.Length; j++)
                {
                    encounters[i][j] = new Image();
                    encounters[i][j].Width = 16;
                    encounters[i][j].Height = 16;
                    encounters[i][j].Source = new BitmapImage(new Uri("Images/Dungeon/Blank.png", UriKind.Relative));
                    LayoutRoot.Children.Add(encounters[i][j]);
                    encounters[i][j].HorizontalAlignment = HorizontalAlignment.Left;
                    encounters[i][j].VerticalAlignment = VerticalAlignment.Top;
                    encounters[i][j].Margin = new Thickness(x_offset, y_offset, 0, 0);
                    encounters[i][j].Visibility = Visibility.Collapsed;
                    x_offset += 128;
                }
                y_offset += 128;
            }
            // Find a blank map piece
            int blank = 0;
            for (int i = 0; i < dungeon_theme.map_pieces.Length; i++)
            {
                if (dungeon_theme.map_pieces[i].is_blank) blank = i;
            }
            // Set up a map of blank squares
            this.map = new int[dungeon_dimensions][];
            for (int i = 0; i < this.map.Length; i++)
            {
                this.map[i] = new int[dungeon_dimensions];
                for (int j = 0; j < this.map.Length; j++)
                {
                    this.map[i][j] = blank;
                }
            }
            if (first_player)
            {
                // Randomly generate a dungeon
                generate_dungeon();
                // Fill it with encounters
                populate_dungeon();
                // Post the dungeon to the server if necessary
                if (!only_player)
                {
                    post_dungeon();
                    // Start polling server for other player locations
                    start_poll_locations();
                }

                // Update the player's current view
                for (int i = 0; i < this.map_dimensions; i++)
                {
                    for (int j = 0; j < this.map_dimensions; j++)
                    {
                        this.current_view[i][j] = map[(player_row - 1 + i)][(player_col - 1 + j)];
                    }
                }
                update_map(current_view);
                handle_events();
                update_location(player_row, player_col);
            }
            else
            {
                // Poll server for results
                // ********************************************************************
                // * Set instance id
                // * Set dungeon_dimensions
                // * Set theme
                // ********************************************************************
                // Once the instance is found and the dungeon is constructed, fetch it
                this.instance = 7;
                Uri path = new Uri("dungeon_get_square.php", UriKind.Relative);
                string parameters = String.Format("instance={0}", this.instance);
                HttpConnection.httpPost(path, parameters, dungeon_recieved);
            }
            // Put borders around the maps
            rec_map_border.Margin = new Thickness(10, 10, 0, 0);
            rec_map_border.Width = map_dimensions * 128;
            rec_map_border.Height = map_dimensions * 128;
            rec_minimap_border.Margin = new Thickness(410, 9, 0, 0);
            rec_minimap_border.Width = (dungeon_dimensions - 2) * 16 + 2;
            rec_minimap_border.Height = (dungeon_dimensions - 2) * 16 + 2;
            img_light_radius.Margin = new Thickness(138, 138, 0, 0);
            img_light_base.Margin = new Thickness(138, 138, 0, 0);
            img_light_base.Source = get_image(dungeon_theme.exit_piece);
            // Set up the key detector
            txt_key.KeyDown += new KeyEventHandler(key_pressed);
            // Focus on the key detector
            txt_key.Focus();
        }

        public UIElement Element { get { return this; } }

        private void enter_dungeon()
        {
            Uri path = new Uri("enter_dungeon.php", UriKind.Relative);
            string parameters = String.Format("instance={0}&x={1}&y={2}&charnum={3}"
                , this.instance, player_row, player_col, this.charid);
            HttpConnection.httpPost(path, parameters, dungeon_sent);
            //MessageBox.Show("Dungeon Entered");
        }

        private void start_poll_locations()
        {
            Uri path = new Uri("dungeon_get_positions.php", UriKind.Relative);
            try
            {
                HttpConnection.httpLongPoll(path, update_player_locations, 2);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void update_player_locations(object sender, DownloadStringCompletedEventArgs e)
        {
            // Update the players location
            enter_dungeon();
            if (e.Error == null)
            {
                XDocument doc = XDocument.Parse(e.Result);
                // Check if any other players were found
                if (doc.Element("notfound") == null)
                {
                    // Yay for class re-use! Shape = charid, content = instance
                    var positions = from position in doc.Descendants("position")
                                    select new MapSquare()
                                    {
                                        x = (int)position.Element("x"),
                                        y = (int)position.Element("y"),
                                        shape = (int)position.Element("char"),
                                        content = (int)position.Element("instance")
                                    };
                    MapSquare[] map_squares = positions.ToArray();
                    // Clear the players array
                    this.players = new int[this.dungeon_dimensions][];
                    for (int i = 0; i < dungeon_dimensions; i++)
                    {
                        this.players[i] = new int[dungeon_dimensions];
                    }
                    // Put in all new player locations
                    for (int i = 0; i < map_squares.Length; i++)
                    {
                        if (map_squares[i].content == this.instance
                            && map_squares[i].shape != this.charid)
                            players[(map_squares[i].x)][(map_squares[i].y)] = 1;
                    }
                }
            }
            else
            {
                MessageBox.Show("Could not recieve player locations: " + e.Error.ToString());
            }
        }

        private void post_dungeon()
        {
            // Post the dungeon info to the database and get the instance id
            string parameters
                = String.Format("theme={0}&size={1}", this.theme, this.dungeon_dimensions);
            Uri path = new Uri("create_dungeon.php", UriKind.Relative);
            HttpConnection.httpPost(path, parameters, send_dungeon);
        }

        private void send_dungeon(object sender, UploadStringCompletedEventArgs e)
        {
            // Use the recieved id to send all squares to the database
            if (e.Error == null)
            {
                XDocument doc = XDocument.Parse(e.Result);
                if (doc.Element("instance") != null)
                {
                    MessageBox.Show("Instance: " + (int)doc.Element("instance"));
                    this.instance = (int)doc.Element("instance");
                    // Update the player location on the server
                    enter_dungeon();
                    // Send the dungeon square by square
                    Uri path = new Uri("dungeon_add_square.php", UriKind.Relative);
                    string parameters;
                    for (int i = 0; i < this.dungeon_dimensions; i++)
                    {
                        for (int j = 0; j < this.dungeon_dimensions; j++)
                        {
                            parameters = String.Format("instance={0}&xcoord={1}&ycoord={2}"
                               + "&shape={3}&content={4}", this.instance, i, j,
                               this.map[i][j], this.events[i][j]);
                            HttpConnection.httpPost(path, parameters, dungeon_sent);
                        }
                    }
                    // Check the dungeon is still fully intact
                    path = new Uri("dungeon_get_square.php", UriKind.Relative);
                    parameters = String.Format("instance={0}", this.instance);
                    HttpConnection.httpPost(path, parameters, dungeon_recieved);
                }
                else
                {
                    MessageBox.Show("FAIL!");
                }
            }
            else
            {
                MessageBox.Show("Could not send dungeon: " + e.Error.ToString());
            }
        }

        private void dungeon_sent(object sender, UploadStringCompletedEventArgs e)
        {
            // Function which does nothing (needs to be replaced)
            if (e.Error == null)
            {
                //MessageBox.Show("Result: " + e.Result);
            }
            else
            {
                MessageBox.Show("Could not send dungeon: " + e.Error.ToString());
            }
        }

        private void dungeon_recieved(object sender, UploadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                try
                {
                    XDocument doc = XDocument.Parse(e.Result);
                    var squares = from square in doc.Descendants("square")
                                  select new MapSquare()
                                  {
                                      x = (int)square.Element("x"),
                                      y = (int)square.Element("y"),
                                      shape = (int)square.Element("shape"),
                                      content = (int)square.Element("content")
                                  };
                    MapSquare[] map_squares = squares.ToArray();
                    // DEBUG
                    bool recheck = false;
                    int[][] map_temp = new int[dungeon_dimensions][];
                    int[][] events_temp = new int[dungeon_dimensions][];
                    int[][] filled = new int[dungeon_dimensions][];
                    for (int i = 0; i < dungeon_dimensions; i++)
                    {
                        filled[i] = new int[dungeon_dimensions];
                        map_temp[i] = new int[dungeon_dimensions];
                        events_temp[i] = new int[dungeon_dimensions];
                    }
                    for (int i = 0; i < map_squares.Length; i++)
                    {
                        int x = map_squares[i].x;
                        int y = map_squares[i].y;
                        filled[x][y] = 1;
                        map_temp[x][y] = map_squares[i].shape;
                        events_temp[x][y] = map_squares[i].content;
                    }
                    for (int i = 0; i < dungeon_dimensions; i++)
                        for (int j = 0; j < dungeon_dimensions; j++)
                            if (!(filled[i][j] == 1))
                            {
                                recheck = true;
                                MessageBox.Show("Unfilled: " + i + "," + j);
                                if (first_player)
                                {
                                    Uri path = new Uri("dungeon_add_square.php", UriKind.Relative);
                                    string parameters = String.Format("instance={0}&xcoord={1}&ycoord={2}"
                                        + "&shape={3}&content={4}", this.instance, i, j,
                                        this.map[i][j], this.events[i][j]);
                                    HttpConnection.httpPost(path, parameters, dungeon_sent);
                                }
                            }
                    if (first_player)
                    {
                        if (!recheck)
                        {
                            // Update the database to say the dungeon has been constructed
                            Uri path = new Uri("dungeon_built.php", UriKind.Relative);
                            string parameters
                              = String.Format("instance={0}", this.instance);
                            HttpConnection.httpPost(path, parameters, dungeon_sent);
                        }
                        else
                        {
                            // Recheck the dungeon in the database
                            Uri path = new Uri("dungeon_get_square.php", UriKind.Relative);
                            string parameters = String.Format("instance={0}", this.instance);
                            HttpConnection.httpPost(path, parameters, dungeon_recieved);
                        }
                    }
                    else
                    {
                        events = events_temp;
                        map = map_temp;
                        // Tell the player where the start location is and set up their map
                        player_row = (dungeon_dimensions / 2);
                        player_col = (dungeon_dimensions / 2);
                        // Update this player's location on the server
                        enter_dungeon();
                        update_current_values();
                        update_map(current_view);
                        handle_events();
                        update_location(player_row, player_col);
                        // Set up the 'other players' array
                        this.players = new int[this.dungeon_dimensions][];
                        for (int i = 0; i < dungeon_dimensions; i++)
                        {
                            this.players[i] = new int[dungeon_dimensions];
                        }
                        // Start polling server for other player locations
                        start_poll_locations();
                    }
                }
                catch (Exception e1)
                {
                    MessageBox.Show("Error in parsing dungeon: " + e1.ToString());
                }
            }
            else
            {
                MessageBox.Show("Could not recieve dungeon: " + e.Error.ToString());
            }
        }

        private void update_map(int[][] new_map)
        {
            map00.Source = get_image(new_map[0][0]);
            map01.Source = get_image(new_map[0][1]);
            map02.Source = get_image(new_map[0][2]);
            map10.Source = get_image(new_map[1][0]);
            map11.Source = get_image(new_map[1][1]);
            map12.Source = get_image(new_map[1][2]);
            map20.Source = get_image(new_map[2][0]);
            map21.Source = get_image(new_map[2][1]);
            map22.Source = get_image(new_map[2][2]);
            img_light_base.Source = map11.Source;
        }

        private BitmapImage get_image(int map_piece)
        {
            return this.dungeon_theme.map_pieces[map_piece].tileset_image;
        }

        private bool can_move_north(int map_piece)
        {
            if (this.dungeon_theme.map_pieces[map_piece].exit_north)
            {
                return true;
            }
            return false;
        }

        private bool can_move_south(int map_piece)
        {
            if (this.dungeon_theme.map_pieces[map_piece].exit_south)
            {
                return true;
            }
            return false;
        }

        private bool can_move_west(int map_piece)
        {
            if (this.dungeon_theme.map_pieces[map_piece].exit_west)
            {
                return true;
            }
            return false;
        }

        private bool can_move_east(int map_piece)
        {
            if (this.dungeon_theme.map_pieces[map_piece].exit_east)
            {
                return true;
            }
            return false;
        }

        private bool must_exit_north(int row, int col)
        {
            int above = map[row - 1][col];
            if (can_move_south(above) && !dungeon_theme.map_pieces[above].is_blank)
            {
                return true;
            }
            return false;
        }

        private bool cannot_exit_north(int row, int col)
        {
            int above = map[row - 1][col];
            if (!can_move_south(above))
            {
                return true;
            }
            return false;
        }

        private bool must_exit_south(int row, int col)
        {
            int below = map[row + 1][col];
            if (can_move_north(below) && !dungeon_theme.map_pieces[below].is_blank)
            {
                return true;
            }
            return false;
        }

        private bool cannot_exit_south(int row, int col)
        {
            int below = map[row + 1][col];
            if (!can_move_north(below))
            {
                return true;
            }
            return false;
        }

        private bool must_exit_east(int row, int col)
        {
            int right = map[row][col + 1];
            if (can_move_west(right) && !dungeon_theme.map_pieces[right].is_blank)
            {
                return true;
            }
            return false;
        }

        private bool cannot_exit_east(int row, int col)
        {
            int right = map[row][col + 1];
            if (!can_move_west(right))
            {
                return true;
            }
            return false;
        }

        private bool must_exit_west(int row, int col)
        {
            int left = map[row][col - 1];
            if (can_move_east(left) && !dungeon_theme.map_pieces[left].is_blank)
            {
                return true;
            }
            return false;
        }

        private bool cannot_exit_west(int row, int col)
        {
            int left = map[row][col - 1];
            if (!can_move_east(left))
            {
                return true;
            }
            return false;
        }

        public void fill_tilesets()
        {
            // Generate the theme
            this.dungeon_theme = new DungeonTheme(theme, light_level);
            // Set up the lighting
            map00.Opacity = dungeon_theme.light_level;
            map01.Opacity = dungeon_theme.light_level;
            map02.Opacity = dungeon_theme.light_level;
            map10.Opacity = dungeon_theme.light_level;
            map11.Opacity = dungeon_theme.light_level;
            map12.Opacity = dungeon_theme.light_level;
            map20.Opacity = dungeon_theme.light_level;
            map21.Opacity = dungeon_theme.light_level;
            map22.Opacity = dungeon_theme.light_level;

        }

        public void create_minimap()
        {
            int minimap_horiz = 411;
            int minimap_vert = 10;

            this.minimap = new Image[(dungeon_dimensions - 2)][];
            for (int i = 0; i < (dungeon_dimensions - 2); i++)
            {
                this.minimap[i] = new Image[(dungeon_dimensions - 2)];
            }

            for (int i = 0; i < (this.dungeon_dimensions - 2); i++)
            {
                for (int j = 0; j < (this.dungeon_dimensions - 2); j++)
                {
                    minimap[i][j] = new Image();
                    minimap[i][j].Width = 16;
                    minimap[i][j].Height = 16;
                    minimap[i][j].Source = new BitmapImage(new Uri("Images/Dungeon/Blank.png", UriKind.Relative));
                    LayoutRoot.Children.Add(minimap[i][j]);
                    minimap[i][j].HorizontalAlignment = HorizontalAlignment.Left;
                    minimap[i][j].VerticalAlignment = VerticalAlignment.Top;
                    minimap[i][j].Margin = new Thickness(minimap_horiz, minimap_vert, 0, 0);
                    minimap_horiz += 16;
                }

                minimap_horiz = 411;
                minimap_vert += 16;
            }
            img_player_mm = new Image();
            img_player_mm.Width = 8;
            img_player_mm.Height = 8;
            img_player_mm.Source = new BitmapImage(new Uri("Images/Dungeon/PlayerMarker.png", UriKind.Relative));
            LayoutRoot.Children.Add(img_player_mm);
            img_player_mm.HorizontalAlignment = HorizontalAlignment.Left;
            img_player_mm.VerticalAlignment = VerticalAlignment.Top;
        }

        public void generate_dungeon()
        {
            // Fill in borders
            for (int i = 0; i < this.map.Length; i++)
            {
                this.map[0][i] = dungeon_theme.border_piece;
                this.map[(this.map.Length - 1)][i] = dungeon_theme.border_piece;
                this.map[i][0] = dungeon_theme.border_piece;
                this.map[i][(this.map.Length - 1)] = dungeon_theme.border_piece;
            }
            // Tell the player where the start location is and set up their map
            player_row = (dungeon_dimensions / 2);
            player_col = (dungeon_dimensions / 2);
            // Add a start location in the middle of the map
            map[player_row][player_col] = this.dungeon_theme.exit_piece;
            events[player_row][player_col] = 2;
            recursive_path((player_row - 1), player_col);
            recursive_path(player_row, (player_col + 1));
            recursive_path(player_row, (player_col - 1));
            recursive_path((player_row + 1), player_col);
        }

        public void recursive_path(int row, int col)
        {
            if (dungeon_theme.map_pieces[(map[row][col])].is_blank)
            {

                int[] available_pieces = new int[this.dungeon_theme.map_pieces.Length];
                // All pieces begin available
                for (int i = 0; i < dungeon_theme.map_pieces.Length; i++)
                {
                    // Don't place borders, blanks or exits
                    if (i != dungeon_theme.border_piece
                        && !dungeon_theme.map_pieces[i].is_blank
                        && i != dungeon_theme.exit_piece) available_pieces[i] = 1;
                }
                // Don't allow pieces to be placed with exits onto borders or misfits
                // North border:
                if ((map[row - 1][col]) == dungeon_theme.border_piece
                    || cannot_exit_north(row, col))
                {
                    for (int i = 0; i < dungeon_theme.map_pieces.Length; i++)
                    {
                        if (dungeon_theme.map_pieces[i].exit_north) available_pieces[i] = 0;
                    }
                }
                // South border:
                if ((map[row + 1][col]) == dungeon_theme.border_piece
                    || cannot_exit_south(row, col))
                {
                    for (int i = 0; i < dungeon_theme.map_pieces.Length; i++)
                    {
                        if (dungeon_theme.map_pieces[i].exit_south) available_pieces[i] = 0;
                    }
                }
                // East border:
                if ((map[row][col + 1]) == dungeon_theme.border_piece
                    || cannot_exit_east(row, col))
                {
                    for (int i = 0; i < dungeon_theme.map_pieces.Length; i++)
                    {
                        if (dungeon_theme.map_pieces[i].exit_east) available_pieces[i] = 0;
                    }
                }
                // West border:
                if ((map[row][col - 1]) == dungeon_theme.border_piece
                    || cannot_exit_west(row, col))
                {
                    for (int i = 0; i < dungeon_theme.map_pieces.Length; i++)
                    {
                        if (dungeon_theme.map_pieces[i].exit_west) available_pieces[i] = 0;
                    }
                }
                // Remove pieces that will not fit
                if (must_exit_north(row, col))
                {
                    for (int i = 0; i < dungeon_theme.map_pieces.Length; i++)
                    {
                        if (!dungeon_theme.map_pieces[i].exit_north) available_pieces[i] = 0;
                    }
                }
                if (must_exit_south(row, col))
                {
                    for (int i = 0; i < dungeon_theme.map_pieces.Length; i++)
                    {
                        if (!dungeon_theme.map_pieces[i].exit_south) available_pieces[i] = 0;
                    }
                }
                if (must_exit_east(row, col))
                {
                    for (int i = 0; i < dungeon_theme.map_pieces.Length; i++)
                    {
                        if (!dungeon_theme.map_pieces[i].exit_east) available_pieces[i] = 0;
                    }
                }
                if (must_exit_west(row, col))
                {
                    for (int i = 0; i < dungeon_theme.map_pieces.Length; i++)
                    {
                        if (!dungeon_theme.map_pieces[i].exit_west) available_pieces[i] = 0;
                    }
                }
                bool found = false;
                int count = 0;
                int index = 0;
                for (int i = 0; i < dungeon_theme.map_pieces.Length; i++)
                {
                    if (available_pieces[i] == 1)
                    {
                        found = true;
                        count++;
                        index = i;
                    }
                }
                if (!found)
                    return;
                else if (count >= 4) // MAGIC NUMBER
                {
                    found = false;
                    while (!found)
                    {
                        index = rnd.Next(dungeon_theme.lower_bound, dungeon_theme.upper_bound);
                        if (available_pieces[index] == 1)
                            found = true;
                    }
                }
                map[row][col] = index;
                this.size++;
                if (can_move_north(index))
                {
                    recursive_path((row - 1), col);
                }
                if (can_move_south(index))
                {
                    recursive_path((row + 1), col);
                }
                if (can_move_west(index))
                {
                    recursive_path(row, (col - 1));
                }
                if (can_move_east(index))
                {
                    recursive_path(row, (col + 1));
                }
            }
        }

        public void populate_dungeon()
        {
            int to_place = dungeon_enemies;
            if (to_place > this.size)
                to_place = this.size - 1;
            while (to_place > 0)
            {
                int row = rnd.Next(1, dungeon_dimensions - 2);
                int col = rnd.Next(1, dungeon_dimensions - 2);
                int piece = map[row][col];
                if (events[row][col] == 0 && !dungeon_theme.map_pieces[piece].is_blank)
                {
                    events[row][col] = 3;
                    to_place--;
                    this.size--;
                }
            }
            to_place = dungeon_items;
            if (to_place > this.size)
                to_place = this.size - 1;
            while (to_place > 0)
            {
                int row = rnd.Next(1, dungeon_dimensions - 2);
                int col = rnd.Next(1, dungeon_dimensions - 2);
                int piece = map[row][col];
                if (events[row][col] == 0 && !dungeon_theme.map_pieces[piece].is_blank)
                {
                    events[row][col] = 4;
                    to_place--;
                    this.size--;
                }
            }
        }

        public void minimap_reveal(int row, int col)
        {
            minimap[row - 1][col - 1].Source
                = this.dungeon_theme.map_pieces[(map[row][col])].minimap_image;
        }

        public void update_location(int row, int col)
        {
            // Updates the position of the player marker on the minimap
            img_player_mm.Margin
                = new Thickness((411 + 4 + (16 * (col - 1))), (10 + 4 + (16 * (row - 1))), 0, 0);
        }

        public void handle_events()
        {
            minimap_reveal(player_row, player_col);
            int relative_row_base = (player_row - 1);
            int relative_col_base = (player_col - 1);
            // Place any images of encounters
            for (int i = relative_row_base; i <= (relative_row_base + 2); i++)
                for (int j = relative_col_base; j <= (relative_col_base + 2); j++)
                {
                    // Other players take priority (so long as the player is not on that square)
                    if (players[i][j] == 1 && (i != player_row || j != player_col))
                    {
                        encounters[(i - relative_row_base)][(j - relative_col_base)].Visibility
                            = Visibility.Visible;
                        encounters[(i - relative_row_base)][(j - relative_col_base)].Source
                            = new BitmapImage(new Uri("Images/Dungeon/player2.png", UriKind.Relative));
                    }
                    else if (events[i][j] == 3)
                    {
                        encounters[(i - relative_row_base)][(j - relative_col_base)].Visibility
                            = Visibility.Visible;
                        encounters[(i - relative_row_base)][(j - relative_col_base)].Source
                            = new BitmapImage(new Uri("Images/Dungeon/enemy.png", UriKind.Relative));
                    }
                    else if (events[i][j] == 4)
                    {
                        encounters[(i - relative_row_base)][(j - relative_col_base)].Visibility
                            = Visibility.Visible;
                        encounters[(i - relative_row_base)][(j - relative_col_base)].Source
                            = new BitmapImage(new Uri("Images/Dungeon/treasure.png", UriKind.Relative));
                    }
                    else
                    {
                        encounters[(i - relative_row_base)][(j - relative_col_base)].Visibility
                            = Visibility.Collapsed;
                    }
                }
            // Check if the square is an exit
            if (events[player_row][player_col] == 2)
            {
                btn_exit.IsEnabled = true;
            }
            else
            {
                btn_exit.IsEnabled = false;
            }
            if (events[player_row][player_col] == 0)
                events[player_row][player_col] = 1;
            if (events[player_row][player_col] > 2)
            {
                // Confirm the event still exists and remove it from the events on the server
                if (!only_player)
                {
                    Uri path = new Uri("dungeon_remove_event.php", UriKind.Relative);
                    string parameters
                      = String.Format("instance={0}&xcoord={1}&ycoord={2}",
                      this.instance, player_row, player_col);
                    HttpConnection.httpPost(path, parameters, handle_encounter);
                }
                else
                {
                    events[player_row][player_col] = 1;
                }

            }
        }

        private void handle_encounter(object sender, UploadStringCompletedEventArgs e)
        {
            events[player_row][player_col] = 1;
            if (e.Error == null)
            {
                // Handle the combat/treasure...
                XDocument doc = XDocument.Parse(e.Result);
                if (doc.Element("encounter") != null)
                {
                    if ((int)doc.Element("encounter") > 2)
                        MessageBox.Show("Event: " + (int)doc.Element("encounter"));
                }
            }
            else
            {
                MessageBox.Show("Could not recieve encounter: " + e.Error.ToString());
            }
        }

        public void update_current_values()
        {
            if (!only_player)
            {
                Uri path = new Uri("dungeon_get_events.php", UriKind.Relative);
                string parameters = String.Format("instance={0}&xcoord={1}&ycoord={2}",
                    this.instance, (player_row - 1), (player_col - 1));
                HttpConnection.httpPost(path, parameters, events_recieved);
            }
            for (int i = 0; i < this.map_dimensions; i++)
            {
                for (int j = 0; j < this.map_dimensions; j++)
                {
                    this.current_view[i][j] = map[(player_row - 1 + i)][(player_col - 1 + j)];
                }
            }
        }

        private void events_recieved(object sender, UploadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                try
                {
                    XDocument doc = XDocument.Parse(e.Result);
                    var encounters = from encounter in doc.Descendants("encounter")
                                     select new MapSquare()
                                     {
                                         x = (int)encounter.Element("x"),
                                         y = (int)encounter.Element("y"),
                                         content = (int)encounter.Element("content")
                                     };
                    MapSquare[] map_squares = encounters.ToArray();
                    for (int i = 0; i < map_squares.Length; i++)
                    {
                        int x = map_squares[i].x;
                        int y = map_squares[i].y;
                        events[x][y] = map_squares[i].content;
                    }
                }
                catch (Exception e1)
                {
                    MessageBox.Show("Error in parsing events: " + e1.ToString());
                }
            }
            else
            {
                MessageBox.Show("Could not recieve events: " + e.Error.ToString());
            }
        }

        private void key_pressed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up) btn_up_Click(null, null);
            else if (e.Key == Key.Down) btn_down_Click(null, null);
            else if (e.Key == Key.Left) btn_left_Click(null, null);
            else if (e.Key == Key.Right) btn_right_Click(null, null);
        }

        private void btn_up_Click(object sender, RoutedEventArgs e)
        {
            if (events[player_row][player_col] >= 3)
                handle_events();
            if (can_move_north(map[player_row][player_col])
                && events[player_row][player_col] < 3)
            {
                player_row--;
                update_current_values();
                update_map(current_view);
                handle_events();
                update_location(player_row, player_col);
            }
            // Focus on the key detector
            txt_key.Focus();
        }

        private void btn_down_Click(object sender, RoutedEventArgs e)
        {
            if (events[player_row][player_col] >= 3)
                handle_events();
            if (can_move_south(map[player_row][player_col])
                && events[player_row][player_col] < 3)
            {
                player_row++;
                update_current_values();
                update_map(current_view);
                handle_events();
                update_location(player_row, player_col);
            }
            // Focus on the key detector
            txt_key.Focus();
        }

        private void btn_left_Click(object sender, RoutedEventArgs e)
        {
            if (events[player_row][player_col] >= 3)
                handle_events();
            if (can_move_west(map[player_row][player_col])
                && events[player_row][player_col] < 3)
            {
                player_col--;
                update_current_values();
                update_map(current_view);
                handle_events();
                update_location(player_row, player_col);
            }
            // Focus on the key detector
            txt_key.Focus();
        }

        private void btn_right_Click(object sender, RoutedEventArgs e)
        {
            if (events[player_row][player_col] >= 3)
                handle_events();
            if (can_move_east(map[player_row][player_col])
                && events[player_row][player_col] < 3)
            {
                player_col++;
                update_current_values();
                update_map(current_view);
                handle_events();
                update_location(player_row, player_col);
            }
            // Focus on the key detector
            txt_key.Focus();
        }

    }

    public class DungeonTheme
    {
        // Name of the theme
        public string theme_name;
        // Array of available map pieces
        public MapPiece[] map_pieces;
        // Double to represent how bright the dungeon is
        public Double light_level;
        // What piece number represents an exit
        public int exit_piece;
        // What piece is a border
        public int border_piece;
        // Range of values for rnd to generate
        public int lower_bound;
        public int upper_bound;

        public DungeonTheme(int theme, Double light_level)
        {
            this.light_level = light_level;
            if (theme == 0)
            {
                this.theme_name = "Sewer";
                this.exit_piece = 18;
                this.border_piece = 0;
                this.lower_bound = 2;
                this.upper_bound = 13;
                this.map_pieces = new MapPiece[19];
                BitmapImage main_pic
                    = new BitmapImage(new Uri("Images/Dungeon/SewerBlank.png", UriKind.Relative));
                BitmapImage mini_pic
                    = new BitmapImage(new Uri("Images/Dungeon/Blank.png", UriKind.Relative));
                map_pieces[0] = new MapPiece(main_pic, mini_pic, false, false, false, false, false);
                map_pieces[1] = new MapPiece(main_pic, mini_pic, true, true, true, true, true);

                main_pic
                    = new BitmapImage(new Uri("Images/Dungeon/SewerStraightHoriz.png", UriKind.Relative));
                mini_pic
                    = new BitmapImage(new Uri("Images/Dungeon/StraightHoriz.png", UriKind.Relative));
                map_pieces[2] = new MapPiece(main_pic, mini_pic, false, false, true, false, true);

                main_pic
                    = new BitmapImage(new Uri("Images/Dungeon/SewerStraightVert.png", UriKind.Relative));
                mini_pic
                    = new BitmapImage(new Uri("Images/Dungeon/StraightVert.png", UriKind.Relative));
                map_pieces[3] = new MapPiece(main_pic, mini_pic, false, true, false, true, false);

                main_pic
                    = new BitmapImage(new Uri("Images/Dungeon/SewerCornerEastNorth.png", UriKind.Relative));
                mini_pic
                    = new BitmapImage(new Uri("Images/Dungeon/CornerEastNorth.png", UriKind.Relative));
                map_pieces[4] = new MapPiece(main_pic, mini_pic, false, true, true, false, false);

                main_pic
                    = new BitmapImage(new Uri("Images/Dungeon/SewerCornerEastSouth.png", UriKind.Relative));
                mini_pic
                    = new BitmapImage(new Uri("Images/Dungeon/CornerEastSouth.png", UriKind.Relative));
                map_pieces[5] = new MapPiece(main_pic, mini_pic, false, false, true, true, false);

                main_pic
                    = new BitmapImage(new Uri("Images/Dungeon/SewerCornerWestSouth.png", UriKind.Relative));
                mini_pic
                    = new BitmapImage(new Uri("Images/Dungeon/CornerWestSouth.png", UriKind.Relative));
                map_pieces[6] = new MapPiece(main_pic, mini_pic, false, false, false, true, true);

                main_pic
                    = new BitmapImage(new Uri("Images/Dungeon/SewerCornerWestNorth.png", UriKind.Relative));
                mini_pic
                    = new BitmapImage(new Uri("Images/Dungeon/CornerWestNorth.png", UriKind.Relative));
                map_pieces[7] = new MapPiece(main_pic, mini_pic, false, true, false, false, true);

                main_pic
                    = new BitmapImage(new Uri("Images/Dungeon/SewerTJunctionSouth.png", UriKind.Relative));
                mini_pic
                    = new BitmapImage(new Uri("Images/Dungeon/TJunctionSouth.png", UriKind.Relative));
                map_pieces[8] = new MapPiece(main_pic, mini_pic, false, false, true, true, true);

                main_pic
                    = new BitmapImage(new Uri("Images/Dungeon/SewerTJunctionEast.png", UriKind.Relative));
                mini_pic
                    = new BitmapImage(new Uri("Images/Dungeon/TJunctionEast.png", UriKind.Relative));
                map_pieces[9] = new MapPiece(main_pic, mini_pic, false, true, true, true, false);

                main_pic
                    = new BitmapImage(new Uri("Images/Dungeon/SewerTJunctionNorth.png", UriKind.Relative));
                mini_pic
                    = new BitmapImage(new Uri("Images/Dungeon/TJunctionNorth.png", UriKind.Relative));
                map_pieces[10] = new MapPiece(main_pic, mini_pic, false, true, true, false, true);

                main_pic
                    = new BitmapImage(new Uri("Images/Dungeon/SewerTJunctionWest.png", UriKind.Relative));
                mini_pic
                    = new BitmapImage(new Uri("Images/Dungeon/TJunctionWest.png", UriKind.Relative));
                map_pieces[11] = new MapPiece(main_pic, mini_pic, false, true, false, true, true);

                main_pic
                    = new BitmapImage(new Uri("Images/Dungeon/Sewer4Way.png", UriKind.Relative));
                mini_pic
                    = new BitmapImage(new Uri("Images/Dungeon/4Way.png", UriKind.Relative));
                map_pieces[12] = new MapPiece(main_pic, mini_pic, false, true, true, true, true);

                main_pic
                    = new BitmapImage(new Uri("Images/Dungeon/SewerStraightHorizLit.png", UriKind.Relative));
                mini_pic
                    = new BitmapImage(new Uri("Images/Dungeon/StraightHoriz.png", UriKind.Relative));
                map_pieces[13] = new MapPiece(main_pic, mini_pic, false, false, true, false, true);

                main_pic
                    = new BitmapImage(new Uri("Images/Dungeon/SewerDeadEndSouth.png", UriKind.Relative));
                mini_pic
                    = new BitmapImage(new Uri("Images/Dungeon/DeadEndSouth.png", UriKind.Relative));
                map_pieces[14] = new MapPiece(main_pic, mini_pic, false, false, false, true, false);

                main_pic
                    = new BitmapImage(new Uri("Images/Dungeon/SewerDeadEndEast.png", UriKind.Relative));
                mini_pic
                    = new BitmapImage(new Uri("Images/Dungeon/DeadEndEast.png", UriKind.Relative));
                map_pieces[15] = new MapPiece(main_pic, mini_pic, false, false, true, false, false);

                main_pic
                    = new BitmapImage(new Uri("Images/Dungeon/SewerDeadEndNorth.png", UriKind.Relative));
                mini_pic
                    = new BitmapImage(new Uri("Images/Dungeon/DeadEndNorth.png", UriKind.Relative));
                map_pieces[16] = new MapPiece(main_pic, mini_pic, false, true, false, false, false);

                main_pic
                    = new BitmapImage(new Uri("Images/Dungeon/SewerDeadEndWest.png", UriKind.Relative));
                mini_pic
                    = new BitmapImage(new Uri("Images/Dungeon/DeadEndWest.png", UriKind.Relative));
                map_pieces[17] = new MapPiece(main_pic, mini_pic, false, false, false, false, true);

                main_pic
                    = new BitmapImage(new Uri("Images/Dungeon/SewerExit.png", UriKind.Relative));
                mini_pic
                    = new BitmapImage(new Uri("Images/Dungeon/Exit.png", UriKind.Relative));
                map_pieces[18] = new MapPiece(main_pic, mini_pic, false, true, true, true, true);

            }
            else if (theme == 1)
            {
                this.theme_name = "Field";
                this.exit_piece = 11;
                this.border_piece = 10;
                this.lower_bound = 9;
                this.upper_bound = 9;
                this.map_pieces = new MapPiece[12];
                BitmapImage main_pic
                    = new BitmapImage(new Uri("Images/Dungeon/FieldOpen.png", UriKind.Relative));
                BitmapImage mini_pic
                    = new BitmapImage(new Uri("Images/Dungeon/Open.png", UriKind.Relative));
                map_pieces[9] = new MapPiece(main_pic, mini_pic, false, true, true, true, true);

                main_pic
                    = new BitmapImage(new Uri("Images/Dungeon/FieldLeft.png", UriKind.Relative));
                mini_pic
                    = new BitmapImage(new Uri("Images/Dungeon/Open.png", UriKind.Relative));
                map_pieces[5] = new MapPiece(main_pic, mini_pic, false, true, true, true, false);

                main_pic
                    = new BitmapImage(new Uri("Images/Dungeon/FieldTop.png", UriKind.Relative));
                mini_pic
                    = new BitmapImage(new Uri("Images/Dungeon/Open.png", UriKind.Relative));
                map_pieces[6] = new MapPiece(main_pic, mini_pic, false, false, true, true, true);

                main_pic
                    = new BitmapImage(new Uri("Images/Dungeon/FieldRight.png", UriKind.Relative));
                mini_pic
                    = new BitmapImage(new Uri("Images/Dungeon/Open.png", UriKind.Relative));
                map_pieces[7] = new MapPiece(main_pic, mini_pic, false, true, false, true, true);

                main_pic
                    = new BitmapImage(new Uri("Images/Dungeon/FieldBottom.png", UriKind.Relative));
                mini_pic
                    = new BitmapImage(new Uri("Images/Dungeon/Open.png", UriKind.Relative));
                map_pieces[8] = new MapPiece(main_pic, mini_pic, false, true, true, false, true);

                main_pic
                    = new BitmapImage(new Uri("Images/Dungeon/FieldBottomLeft.png", UriKind.Relative));
                mini_pic
                    = new BitmapImage(new Uri("Images/Dungeon/Open.png", UriKind.Relative));
                map_pieces[1] = new MapPiece(main_pic, mini_pic, false, true, true, false, false);

                main_pic
                    = new BitmapImage(new Uri("Images/Dungeon/FieldTopLeft.png", UriKind.Relative));
                mini_pic
                    = new BitmapImage(new Uri("Images/Dungeon/Open.png", UriKind.Relative));
                map_pieces[2] = new MapPiece(main_pic, mini_pic, false, false, true, true, false);

                main_pic
                    = new BitmapImage(new Uri("Images/Dungeon/FieldTopRight.png", UriKind.Relative));
                mini_pic
                    = new BitmapImage(new Uri("Images/Dungeon/Open.png", UriKind.Relative));
                map_pieces[3] = new MapPiece(main_pic, mini_pic, false, false, false, true, true);

                main_pic
                    = new BitmapImage(new Uri("Images/Dungeon/FieldBottomRight.png", UriKind.Relative));
                mini_pic
                    = new BitmapImage(new Uri("Images/Dungeon/Open.png", UriKind.Relative));
                map_pieces[4] = new MapPiece(main_pic, mini_pic, false, true, false, false, true);

                main_pic
                    = new BitmapImage(new Uri("Images/Dungeon/SewerBlank.png", UriKind.Relative));
                mini_pic
                    = new BitmapImage(new Uri("Images/Dungeon/Blank.png", UriKind.Relative));
                map_pieces[0] = new MapPiece(main_pic, mini_pic, true, true, true, true, true);

                main_pic
                    = new BitmapImage(new Uri("Images/Dungeon/SewerBlank.png", UriKind.Relative));
                mini_pic
                    = new BitmapImage(new Uri("Images/Dungeon/Open.png", UriKind.Relative));
                map_pieces[10] = new MapPiece(main_pic, mini_pic, false, false, false, false, false);

                main_pic
                    = new BitmapImage(new Uri("Images/Dungeon/FieldOpen.png", UriKind.Relative));
                mini_pic
                    = new BitmapImage(new Uri("Images/Dungeon/Open.png", UriKind.Relative));
                map_pieces[11] = new MapPiece(main_pic, mini_pic, false, true, true, true, true);

            }
            else
            {
                MessageBox.Show("Not a recognized theme!");
            }
        }
    }

    public class MapSquare
    {
        public int x;
        public int y;
        public int shape;
        public int content;
    }

    public class MapPiece
    {
        // How the piece will appear in the main map
        public BitmapImage tileset_image;
        // How it will appear on the minimap
        public BitmapImage minimap_image;
        // Which directions the piece can connect to
        public bool is_blank;
        public bool exit_north;
        public bool exit_east;
        public bool exit_south;
        public bool exit_west;

        public MapPiece(BitmapImage main_img, BitmapImage mini_img, bool blank,
                        bool north, bool east, bool south, bool west)
        {
            this.tileset_image = main_img;
            this.minimap_image = mini_img;
            this.is_blank = blank;
            this.exit_north = north;
            this.exit_east = east;
            this.exit_south = south;
            this.exit_west = west;
        }
    }
}
