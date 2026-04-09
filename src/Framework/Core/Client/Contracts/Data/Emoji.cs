namespace Crudspa.Framework.Core.Client.Contracts.Data;

public class Emoji
{
    public String Character { get; set; } = String.Empty;
    public String Name { get; set; } = String.Empty;

    public static List<Emoji> Reactions()
    {
        return
        [
            new()
            {
                Character = Char.ConvertFromUtf32(0x2764) + Char.ConvertFromUtf32(0xFE0F),
                Name = "Red Heart",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F642),
                Name = "Slightly Smiling Face",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F614),
                Name = "Pensive Face",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F620),
                Name = "Angry Face",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F632),
                Name = "Astonished Face",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F62D),
                Name = "Loudly Crying Face",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F602),
                Name = "Face with Tears of Joy",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F44D),
                Name = "Thumbs Up",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F44E),
                Name = "Thumbs Down",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F44F),
                Name = "Clapping Hands",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x270B),
                Name = "Raised Hand",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F4A1),
                Name = "Light Bulb",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x2714),
                Name = "Check Mark",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x2753),
                Name = "Question Mark",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x2757),
                Name = "Exclamation Mark",
            },
        ];
    }

    public static List<Emoji> Feelings()
    {
        return
        [
            new()
            {
                Character = Char.ConvertFromUtf32(0x2764) + Char.ConvertFromUtf32(0xFE0F),
                Name = "Love It",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F60D),
                Name = "Heart Eyes",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F604),
                Name = "Grinning Face with Smiling Eyes",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F642),
                Name = "Slightly Smiling Face",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F60A),
                Name = "Smiling Face",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F44D),
                Name = "Thumbs Up",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F610),
                Name = "Neutral Face",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F614),
                Name = "Pensive Face",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F641),
                Name = "Slightly Frowning Face",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F44E),
                Name = "Thumbs Down",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F61E),
                Name = "Disappointed Face",
            },
        ];
    }

    public static List<Emoji> FunAvatars()
    {
        return
        [
            new()
            {
                Character = Char.ConvertFromUtf32(0x1F435),
                Name = "Monkey Face",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F412),
                Name = "Monkey",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F98D),
                Name = "Gorilla",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F9A7),
                Name = "Otter",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F436),
                Name = "Dog Face",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F415),
                Name = "Dog",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F9AE),
                Name = "Guide Dog",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F429),
                Name = "Poodle",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F43A),
                Name = "Wolf",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F98A),
                Name = "Fox",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F99D),
                Name = "Raccoon",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F431),
                Name = "Cat Face",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F408),
                Name = "Cat",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F981),
                Name = "Lion",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F42F),
                Name = "Tiger Face",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F405),
                Name = "Tiger",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F406),
                Name = "Leopard",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F434),
                Name = "Horse Face",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F40E),
                Name = "Horse",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F984),
                Name = "Unicorn",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F993),
                Name = "Zebra",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F98C),
                Name = "Deer",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F42E),
                Name = "Cow Face",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F402),
                Name = "Ox",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F403),
                Name = "Water Buffalo",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F404),
                Name = "Cow",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F437),
                Name = "Pig Face",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F416),
                Name = "Pig",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F417),
                Name = "Boar",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F43D),
                Name = "Pig Nose",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F40F),
                Name = "Ram",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F411),
                Name = "Ewe",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F410),
                Name = "Goat",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F42A),
                Name = "Camel",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F42B),
                Name = "Two-Hump Camel",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F999),
                Name = "Llama",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F992),
                Name = "Giraffe",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F418),
                Name = "Elephant",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F98F),
                Name = "Rhinoceros",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F99B),
                Name = "Hippopotamus",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F42D),
                Name = "Mouse Face",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F401),
                Name = "Mouse",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F400),
                Name = "Rat",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F439),
                Name = "Hamster",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F430),
                Name = "Rabbit Face",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F407),
                Name = "Rabbit",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F43F),
                Name = "Chipmunk",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F994),
                Name = "Hedgehog",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F987),
                Name = "Bat",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F43B),
                Name = "Bear",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F428),
                Name = "Koala",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F43C),
                Name = "Panda",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F9A5),
                Name = "Sloth",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F9A6),
                Name = "Otter",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F9A8),
                Name = "Skunk",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F998),
                Name = "Kangaroo",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F9A1),
                Name = "Badger",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F43E),
                Name = "Paw Prints",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F983),
                Name = "Turkey",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F414),
                Name = "Chicken",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F413),
                Name = "Rooster",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F423),
                Name = "Hatching Chick",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F424),
                Name = "Baby Chick",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F425),
                Name = "Front-Facing Baby Chick",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F426),
                Name = "Bird",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F427),
                Name = "Penguin",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F54A),
                Name = "Dove",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F985),
                Name = "Eagle",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F986),
                Name = "Duck",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F9A2),
                Name = "Swan",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F989),
                Name = "Owl",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F9A9),
                Name = "Flamingo",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F99A),
                Name = "Peacock",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F99C),
                Name = "Parrot",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F438),
                Name = "Frog",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F40A),
                Name = "Crocodile",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F422),
                Name = "Turtle",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F98E),
                Name = "Lizard",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F40D),
                Name = "Snake",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F432),
                Name = "Dragon Face",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F409),
                Name = "Dragon",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F995),
                Name = "T-Rex",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F996),
                Name = "Sauropod",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F433),
                Name = "Spouting Whale",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F40B),
                Name = "Whale",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F42C),
                Name = "Dolphin",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F41F),
                Name = "Fish",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F420),
                Name = "Tropical Fish",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F421),
                Name = "Blowfish",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F988),
                Name = "Shark",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F419),
                Name = "Octopus",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F41A),
                Name = "Spiral Shell",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F40C),
                Name = "Snail",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F98B),
                Name = "Butterfly",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F41B),
                Name = "Bug",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F41C),
                Name = "Ant",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F41D),
                Name = "Honeybee",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F41E),
                Name = "Lady Beetle",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F997),
                Name = "Cricket",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F577),
                Name = "Spider",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F578),
                Name = "Spider Web",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F982),
                Name = "Scorpion",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F99F),
                Name = "Mosquito",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F9A0),
                Name = "Microbe",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F980),
                Name = "Crab",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F99E),
                Name = "Lobster",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F990),
                Name = "Shrimp",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F991),
                Name = "Squid",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F921),
                Name = "Clown Face",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F47B),
                Name = "Ghost",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F47D),
                Name = "Alien",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F916),
                Name = "Robot",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F338),
                Name = "Cherry Blossom",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F339),
                Name = "Rose",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F33B),
                Name = "Sunflower",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F33C),
                Name = "Blossom",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F332),
                Name = "Evergreen Tree",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F333),
                Name = "Deciduous Tree",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F334),
                Name = "Palm Tree",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F335),
                Name = "Cactus",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F337),
                Name = "Tulip",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F340),
                Name = "Four Leaf Clover",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F341),
                Name = "Maple Leaf",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x2603),
                Name = "Snowman",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F383),
                Name = "Jack-O-Lantern",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x26BD),
                Name = "Soccer Ball",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x26BE),
                Name = "Baseball",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F94E),
                Name = "Softball",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F3C0),
                Name = "Basketball",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F3D0),
                Name = "Volleyball",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F3C8),
                Name = "American Football",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F3BE),
                Name = "Tennis",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F3AE),
                Name = "Video Game",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F579),
                Name = "Joystick",
            },

            new()
            {
                Character = Char.ConvertFromUtf32(0x1F9F8),
                Name = "Teddy Bear",
            },
        ];
    }
}