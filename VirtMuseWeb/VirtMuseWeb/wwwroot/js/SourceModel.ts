export class Source {
    ID: number;
    Name: string;
    URL: string;

    constructor(name: string, url: string)
    {
        this.ID = -1;
        this.Name = name;
        this.URL = url;
    }

}