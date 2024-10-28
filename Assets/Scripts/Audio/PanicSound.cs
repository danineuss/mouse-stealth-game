namespace Audio
{
    public class PanicSound
    {
        public string Name { get; private set; }

        private PanicSound(string name) { Name = name; }

        public static PanicSound ScaredOne => new PanicSound("ScaredOne");
        public static PanicSound ScaredTwo => new PanicSound("ScaredTwo");
        public static PanicSound ScaredThree => new PanicSound("ScaredThree");
        public static PanicSound Panicking => new PanicSound("Panicking");
    }
}