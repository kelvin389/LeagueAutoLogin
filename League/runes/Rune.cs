namespace League
{
    class Rune
    {
        // TODO: accessor methods
        public int id;
        public string iconPath;
        public string name;
        public string key;

        public Rune(int id, string path, string name, string key)
        {
            this.id = id;
            this.iconPath = path;
            this.name = name;
            this.key = key;
        }
    }
}
