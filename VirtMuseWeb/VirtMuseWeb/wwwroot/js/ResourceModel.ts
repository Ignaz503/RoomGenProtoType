import MetaDataModel = require('MetaDataModel');

export enum Type {
    Image = 0,
    Mesh = 1,
    RoomStyle = 2
};

export class Resource {
    ID: number;
    Type: Type;
    MetaData: MetaDataModel.MetaData;
    Interaction: string;
    Data: string[];
}