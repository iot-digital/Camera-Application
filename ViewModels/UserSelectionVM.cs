namespace ParkingDemo.ViewModels
{
    public class UserSelectionVM
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsSelected { get; set; }

        public UserSelectionVM()
        {
        }

        public UserSelectionVM(int id, string name, bool isSelected)
        {
            Id = id;
            Name = name;
            IsSelected = isSelected;
        }
    }
}