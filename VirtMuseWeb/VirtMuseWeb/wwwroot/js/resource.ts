import * as ko from "../lib/knockout/knockout";
import $ = require("jquery");
import Model = require('../js/ResourceModel');
import CreatorModel = require('../js/CreatorModel');
import SourceModel = require('../js/SourceModel');
import { Type } from "../js/ResourceModel";

class ResourceViewModel {
    resType: { num: number, name: string }[] =
        [{ num: Model.Type.Image.valueOf(), name: 'Image' },
            { num: Model.Type.Mesh.valueOf(), name: 'Mesh' },
            { num: Model.Type.RoomStyle.valueOf(),name:"Room Style" }];

    resource: KnockoutObservable<Model.Resource> = new ko.observable();

    creators: KnockoutObservableArray<{creator: CreatorModel.Creator
        , isDead: KnockoutObservable<boolean>
    }> = new ko.observableArray([{ creator: new CreatorModel.Creator("", "", ""),isDead:new ko.observable(false) }]);

    sources: KnockoutObservableArray<SourceModel.Source> = new ko.observableArray([new SourceModel.Source("","")]);

    file: KnockoutObservable<string> = new ko.observable();

    type: KnockoutObservable<number> = new ko.observable();

    upload: () => void;//uploads file via ajax post, validates input, adds sources and creators to resource 

    addCreator: () => void;//adds creator to ko observable creators

    removeCreator: (creator) => void; // removes creator from creators ko observable

    addSource: () => void;//adds a source to ko observable

    removeSource: (src) => void;//removes a source from sources ko observable

    openFile: (insertIdx: number, element, event) => void;//opens any file and adds to resource data

    openImage: (element, event) => void;//oepns an image

    openMesh: (element, event) => void;//oepns a .obj file

    openMaterial: (element, event) => void;//opens a material for a mesh

    openTexture: (element, event) => void;//opens a texture for a mesh

    openFloorTexture: (element, event) => void;//opens a texture for a mesh

    openCeilingTexture: (element, event) => void;//opens a texture for a mesh

    openWallTexture: (element, event) => void;//opens a texture for a mesh

    validateInput: () => boolean;//validates the user input TODO: sql injection check

    typeChange: (val) => void; // event for upload type change that updates ko observable

    test: () => void;

    constructor()
    {
        this.resource({
            ID: -1,
            Type: Model.Type.Image,
            MetaData: {
                ResourceID: -1,
                NameOfPiece: "",
                DateOfCreation: "",
                Creators: [],
                FurtherInformation: "",
                License: "",
                Sources: [],
            },
            Data: []
        }); //initialize resource

        this.type(0);//initialize type

        this.upload = () => {

            var i: number;
            for (i = 0; i < this.creators().length; i++)
            {
                if (!this.creators()[i].isDead())
                {
                    this.creators()[i].creator.DateOfDeath = "";
                }
                this.resource().MetaData.Creators.push(this.creators()[i].creator);

            }

            for (i = 0; i < this.sources().length; i++)
            {
                this.resource().MetaData.Sources.push(this.sources()[i]);
            }

            if (!this.validateInput())
                return;
            $.ajax(
                {
                    url: "/api/resource/postresource",
                    type: "POST",
                    contentType: 'application/json; charset=utf-8',
                    dataType: "json",
                    data: JSON.stringify(this.resource()),
                    success: (data) => { alert(data); },
                    error: (xhr, resp, text) => {
                        console.log(xhr, resp, text);
                    }
                });
        };

        this.addCreator = () => {
            this.creators.push({ creator: new CreatorModel.Creator("", "", ""), isDead:new ko.observable(false) });
        }

        this.removeCreator = (creator) =>
        {
            if (this.creators().length == 1)
            {
                alert("you need at least one creator");
                return;
            }
            this.creators.remove(creator);
        } 

        this.addSource = () =>
        {
            this.sources.push(new SourceModel.Source("", ""));
        }

        this.removeSource = (src) =>
        {
            if (this.sources().length == 1)
            {
                alert("You need at leaste one source");
                return;//at atleast one ressources
            }
            this.sources.remove(src);
        }

        this.openFile = (insertIdx: number, element, event) =>
        {
            var file = event.target.files[0];
            var f = new FileReader();

            f.onload = (e) => {
                var res: string = f.result;
                //remove data:*/*;base64, aaat the start of string
                res = res.slice(res.indexOf(",") + 1, res.length);
                this.resource().Data.splice(insertIdx, 0, res);
            };
            f.onerror = (e) => {
                alert("Somthing went wrong whilst reading the file, pleas try again\n" +
                    e);
            };
            f.onprogress = (e) => {
                //TODO progress update
            };
            if (file) {
                f.readAsDataURL(file);
            }
           
        }

        this.validateInput = () => {
            var i: number = 0;// loop var

            if (this.resource().Type != Type.RoomStyle)
            {
                if (this.resource().MetaData.Creators.length < 1) {
                    alert("Please enter at least one creator");
                    return false;
                } 
                for (i = 0; i < this.resource().MetaData.Creators.length; i++)
                {
                    if (this.resource().MetaData.Creators[i].Name == "")
                    {
                        alert("Please give every creator a name!");
                        return false;
                    }
                }
            }

            if (this.resource().MetaData.Sources.length < 1)
            {
                alert("Pleas enter at least one source");
                return false;
            }
            for (i = 0; i < this.resource().MetaData.Sources.length; i++)
            {
                if (this.resource().MetaData.Sources[i].Name == "")
                {
                    alert("Please at least all name your resources");
                    return false;
                }
            }

            if (this.resource().MetaData.License == "")
            {
                alert("Please enter the license under which this/these resource/s stand(s)");
                return false;
            }
            if (this.resource().Data == null)
            {
                if (this.resource().Type == Type.Image) alert("Please choose a image to upload");
                else if (this.resource().Type == Type.Mesh) alert("Please choos at least a mesh to upload");
                else if (this.resource().Type == Type.RoomStyle) alert("Please choose 3 images to upload");
                return false;
            }

            //mesh check
            if (this.resource().Data[0] == "")
            {
                alert("You need to choose a mesh to upload");
                return false;
            }

            //style echck
            if (this.resource().Data[0] == "" ||
                this.resource().Data[1] == "" ||
                this.resource().Data[2] == "")
            {
                alert("All three images must be chosen");
                return false;
            }

            return true;
        };

        this.typeChange = (val) => { this.type(val.Type); };

        this.openImage = (element, event) =>
        {
            this.openFile(0, element, event);
        }
            
        this.openMesh = (element, event) =>
        {
            this.openFile(0, element, event);
        }

        this.openMaterial = (element, event) =>
        {
            this.openFile(1, element, event);
        }

        this.openTexture = (element, event) =>
        {
            this.openFile(2, element, event);
        }

        this.openFloorTexture = (element, event) =>
        {
            this.openFile(0, element, event);
        }

        this.openCeilingTexture = (element, event) =>
        {
            this.openFile(1, element, event);
        }

        this.openWallTexture = (element, event) =>
        {
            this.openFile(2, element, event);
        }

        this.test = () => {
            console.log("Hi");
            alert("Hi");
            $.ajax(
                {
                    url: "/api/museum/getmuseum",
                    type: "POST",
                    contentType: 'application/json; charset=utf-8',
                    dataType: "json",
                    data: JSON.stringify({ "MuseumType": "just anything really", "Size": 5 }),
                    success: (data) => { console.log(data); },
                    error: (xhr, resp, text) => {
                        console.log(xhr, resp, text);
                    }
                });
        }
    }
}

ko.applyBindings(new ResourceViewModel());