export class Creator {
    ID: number;
    Name: string;
    DateOfBirth: string;
    DateOfDeath: string;
    constructor(name:string,dateB:string,dateD:string)
    {
        this.ID = -1;
        this.Name = name;
        this.DateOfBirth = dateB;
        this.DateOfDeath = dateD;
    }
}