import CreatorModel = require('CreatorModel');
import SourceModel = require('SourceModel');

export class MetaData {
    ResourceID: number;
    NameOfPiece: string;
    DateOfCreation: string;
    Creators: CreatorModel.Creator[];
    FurtherInformation: string;
    Sources: SourceModel.Source[];
    License: string;
}