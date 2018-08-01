import MetaDataModel = require('MetaDataModel');

export enum Type {
    Image = 0,
    Mesh = 1
};

export class Resource {
    ID: number;
    Type: Type;
    MetaData: MetaDataModel.MetaData;
    Data: any[];
}