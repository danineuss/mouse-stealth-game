namespace Audio
{
    public class EnemySound
    {
        public string Name { get; private set; }

        private EnemySound(string name) { Name = name; }

        public static EnemySound Idle => new EnemySound("Idle");
        public static EnemySound Searching => new EnemySound("Searching");
        public static EnemySound Distracted => new EnemySound("Distracted");
        public static EnemySound Alarmed => new EnemySound("Alarmed");
    }
}