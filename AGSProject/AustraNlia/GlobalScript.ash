// Main header script - this will be included into every script in
// the game (local and global). Do not place functions here; rather,
// place import definitions and #define names here to be used by all
// scripts.

//Variables and structs
struct character_struct
{
    bool is_player;
};

character_struct playable_characters[3];

