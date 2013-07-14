namespace NIHEI.SC4Buddy.Entities.Remote
{
    public partial class User
    {
        public bool IsDeveloper
        {
            get
            {
                return Rights.Equals("Developer");
            }
        }

        public bool IsAuthor
        {
            get
            {
                return Rights.Equals("Author");
            }
        }
    }
}
