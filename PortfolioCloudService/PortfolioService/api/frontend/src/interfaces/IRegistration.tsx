interface IRegistration {
    Name: string;
    LastName: string;
    Address: string;
    City: string;
    Country: string;
    Number: string;
    Email: string;
    Password: string;
    file: File | null;
};

export default IRegistration;