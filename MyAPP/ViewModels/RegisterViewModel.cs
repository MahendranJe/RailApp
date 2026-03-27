using System.ComponentModel.DataAnnotations;

namespace MyAPP.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Gender is required")]
        [Display(Name = "Gender")]
        public string Gender { get; set; } = string.Empty;

        [Required(ErrorMessage = "State is required")]
        [Display(Name = "State")]
        public string State { get; set; } = string.Empty;

        [Required(ErrorMessage = "City/District is required")]
        [Display(Name = "City/District")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Phone number must be exactly 10 digits")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
        [RegularExpression(@"^[a-zA-Z0-9@_]+$", ErrorMessage = "Username can only contain letters, numbers, @ and underscore")]
        [Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d).{6,}$", ErrorMessage = "Password must have at least one uppercase letter and one number")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm password is required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        // Static lists for dropdowns
        public static List<string> GenderList => new List<string> { "Male", "Female", "Other" };

        public static List<string> StateList => new List<string>
        {
            "Andhra Pradesh", "Arunachal Pradesh", "Assam", "Bihar", "Chhattisgarh",
            "Goa", "Gujarat", "Haryana", "Himachal Pradesh", "Jharkhand",
            "Karnataka", "Kerala", "Madhya Pradesh", "Maharashtra", "Manipur",
            "Meghalaya", "Mizoram", "Nagaland", "Odisha", "Punjab",
            "Rajasthan", "Sikkim", "Tamil Nadu", "Telangana", "Tripura",
            "Uttar Pradesh", "Uttarakhand", "West Bengal",
            "Andaman and Nicobar Islands", "Chandigarh", "Dadra and Nagar Haveli and Daman and Diu",
            "Delhi", "Jammu and Kashmir", "Ladakh", "Lakshadweep", "Puducherry"
        };

        public static Dictionary<string, List<string>> CityList => new Dictionary<string, List<string>>
        {
            { "Andhra Pradesh", new List<string> { "Visakhapatnam", "Vijayawada", "Guntur", "Nellore", "Kurnool", "Tirupati", "Kakinada", "Rajahmundry", "Anantapur", "Kadapa" } },
            { "Arunachal Pradesh", new List<string> { "Itanagar", "Naharlagun", "Pasighat", "Tawang", "Ziro", "Bomdila", "Along", "Tezu", "Namsai", "Changlang" } },
            { "Assam", new List<string> { "Guwahati", "Silchar", "Dibrugarh", "Jorhat", "Nagaon", "Tinsukia", "Tezpur", "Bongaigaon", "Karimganj", "Sibsagar" } },
            { "Bihar", new List<string> { "Patna", "Gaya", "Bhagalpur", "Muzaffarpur", "Purnia", "Darbhanga", "Bihar Sharif", "Arrah", "Begusarai", "Katihar" } },
            { "Chhattisgarh", new List<string> { "Raipur", "Bhilai", "Bilaspur", "Korba", "Durg", "Rajnandgaon", "Raigarh", "Jagdalpur", "Ambikapur", "Dhamtari" } },
            { "Goa", new List<string> { "Panaji", "Margao", "Vasco da Gama", "Mapusa", "Ponda", "Bicholim", "Curchorem", "Sanquelim", "Cuncolim", "Quepem" } },
            { "Gujarat", new List<string> { "Ahmedabad", "Surat", "Vadodara", "Rajkot", "Bhavnagar", "Jamnagar", "Junagadh", "Gandhinagar", "Anand", "Nadiad" } },
            { "Haryana", new List<string> { "Faridabad", "Gurgaon", "Panipat", "Ambala", "Yamunanagar", "Rohtak", "Hisar", "Karnal", "Sonipat", "Panchkula" } },
            { "Himachal Pradesh", new List<string> { "Shimla", "Dharamshala", "Mandi", "Solan", "Nahan", "Bilaspur", "Hamirpur", "Una", "Kullu", "Chamba" } },
            { "Jharkhand", new List<string> { "Ranchi", "Jamshedpur", "Dhanbad", "Bokaro", "Deoghar", "Hazaribagh", "Giridih", "Ramgarh", "Dumka", "Phusro" } },
            { "Karnataka", new List<string> { "Bangalore", "Mysore", "Mangalore", "Hubli", "Belgaum", "Gulbarga", "Davangere", "Bellary", "Shimoga", "Tumkur" } },
            { "Kerala", new List<string> { "Thiruvananthapuram", "Kochi", "Kozhikode", "Thrissur", "Kollam", "Alappuzha", "Palakkad", "Kannur", "Kottayam", "Malappuram" } },
            { "Madhya Pradesh", new List<string> { "Indore", "Bhopal", "Jabalpur", "Gwalior", "Ujjain", "Sagar", "Dewas", "Satna", "Ratlam", "Rewa" } },
            { "Maharashtra", new List<string> { "Mumbai", "Pune", "Nagpur", "Thane", "Nashik", "Aurangabad", "Solapur", "Kolhapur", "Amravati", "Navi Mumbai" } },
            { "Manipur", new List<string> { "Imphal", "Thoubal", "Bishnupur", "Churachandpur", "Kakching", "Senapati", "Ukhrul", "Tamenglong", "Chandel", "Jiribam" } },
            { "Meghalaya", new List<string> { "Shillong", "Tura", "Jowai", "Nongstoin", "Williamnagar", "Baghmara", "Resubelpara", "Mairang", "Nongpoh", "Cherrapunji" } },
            { "Mizoram", new List<string> { "Aizawl", "Lunglei", "Champhai", "Serchhip", "Kolasib", "Lawngtlai", "Mamit", "Saiha", "Saitual", "Khawzawl" } },
            { "Nagaland", new List<string> { "Kohima", "Dimapur", "Mokokchung", "Tuensang", "Wokha", "Zunheboto", "Mon", "Phek", "Longleng", "Peren" } },
            { "Odisha", new List<string> { "Bhubaneswar", "Cuttack", "Rourkela", "Berhampur", "Sambalpur", "Puri", "Balasore", "Bhadrak", "Baripada", "Jharsuguda" } },
            { "Punjab", new List<string> { "Ludhiana", "Amritsar", "Jalandhar", "Patiala", "Bathinda", "Mohali", "Pathankot", "Hoshiarpur", "Batala", "Moga" } },
            { "Rajasthan", new List<string> { "Jaipur", "Jodhpur", "Udaipur", "Kota", "Bikaner", "Ajmer", "Bhilwara", "Alwar", "Sikar", "Sri Ganganagar" } },
            { "Sikkim", new List<string> { "Gangtok", "Namchi", "Geyzing", "Mangan", "Rangpo", "Singtam", "Jorethang", "Nayabazar", "Ravangla", "Pelling" } },
            { "Tamil Nadu", new List<string> { "Chennai", "Coimbatore", "Madurai", "Tiruchirappalli", "Salem", "Tirunelveli", "Tiruppur", "Vellore", "Erode", "Thoothukkudi" } },
            { "Telangana", new List<string> { "Hyderabad", "Warangal", "Nizamabad", "Karimnagar", "Khammam", "Ramagundam", "Mahbubnagar", "Nalgonda", "Adilabad", "Suryapet" } },
            { "Tripura", new List<string> { "Agartala", "Udaipur", "Dharmanagar", "Kailashahar", "Ambassa", "Belonia", "Khowai", "Teliamura", "Sabroom", "Sonamura" } },
            { "Uttar Pradesh", new List<string> { "Lucknow", "Kanpur", "Ghaziabad", "Agra", "Varanasi", "Meerut", "Allahabad", "Bareilly", "Aligarh", "Moradabad" } },
            { "Uttarakhand", new List<string> { "Dehradun", "Haridwar", "Roorkee", "Haldwani", "Rudrapur", "Kashipur", "Rishikesh", "Nainital", "Mussoorie", "Pithoragarh" } },
            { "West Bengal", new List<string> { "Kolkata", "Howrah", "Durgapur", "Asansol", "Siliguri", "Bardhaman", "Malda", "Kharagpur", "Haldia", "Raiganj" } },
            { "Andaman and Nicobar Islands", new List<string> { "Port Blair", "Diglipur", "Rangat", "Mayabunder", "Car Nicobar", "Wandoor", "Bamboo Flat", "Garacharma", "Prothrapur", "Ferrargunj" } },
            { "Chandigarh", new List<string> { "Chandigarh", "Manimajra", "Burail", "Daria", "Mauli Jagran", "Behlana", "Hallomajra", "Sector 17", "Sector 22", "Sector 35" } },
            { "Dadra and Nagar Haveli and Daman and Diu", new List<string> { "Silvassa", "Daman", "Diu", "Amli", "Naroli", "Vapi", "Khanvel", "Dunetha", "Samarvarni", "Dadra" } },
            { "Delhi", new List<string> { "New Delhi", "Central Delhi", "South Delhi", "North Delhi", "East Delhi", "West Delhi", "Shahdara", "Dwarka", "Rohini", "Lajpat Nagar" } },
            { "Jammu and Kashmir", new List<string> { "Srinagar", "Jammu", "Anantnag", "Baramulla", "Udhampur", "Sopore", "Kathua", "Kupwara", "Pulwama", "Rajouri" } },
            { "Ladakh", new List<string> { "Leh", "Kargil", "Diskit", "Turtuk", "Nubra", "Zanskar", "Padum", "Drass", "Sankoo", "Hanle" } },
            { "Lakshadweep", new List<string> { "Kavaratti", "Agatti", "Minicoy", "Amini", "Andrott", "Kalpeni", "Kadmat", "Kiltan", "Chetlat", "Bitra" } },
            { "Puducherry", new List<string> { "Puducherry", "Karaikal", "Mahe", "Yanam", "Ozhukarai", "Villianur", "Bahour", "Ariyankuppam", "Mannadipet", "Nettapakkam" } }
        };
    }

    public class UserListViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Gender { get; set; }
        public string? State { get; set; }
        public string? City { get; set; }
        public string? Username { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
    }
}
