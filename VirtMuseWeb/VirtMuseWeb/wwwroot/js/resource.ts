import * as ko from "../lib/knockout/knockout";
import $ = require("jquery");
import Model = require('../js/ResourceModel');
import CreatorModel = require('../js/CreatorModel');
import SourceModel = require('../js/SourceModel');

class ResourceViewModel {
    resType: { num: number, name: string }[] = [{ num: Model.Type.Image.valueOf(), name: 'Image' },
    { num: Model.Type.Mesh.valueOf(), name: 'Mesh' }];

    resource: KnockoutObservable<Model.Resource> = new ko.observable();

    creators: KnockoutObservableArray<{creator: CreatorModel.Creator
        , isDead: KnockoutObservable<boolean>
    }> = new ko.observableArray([{ creator: new CreatorModel.Creator("", "", ""),isDead:new ko.observable(false) }]);

    sources: KnockoutObservableArray<SourceModel.Source> = new ko.observableArray([new SourceModel.Source("","")]);

    file: KnockoutObservable<string> = new ko.observable();

    upload: () => void;

    addCreator: () => void;

    removeCreator: (creator) => void;

    addSource: () => void;

    removeSource: (src) => void;

    openFile: (element, event) => void;

    validateInput: () => boolean;

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
                License:"",
                Sources: [],
            },
            Data : null
        });

        this.upload = () => {

            var i: number;
            //var c: string = "";
            for (i = 0; i < this.creators().length; i++)
            {
                if (!this.creators()[i].isDead())
                {
                    this.creators()[i].creator.DateOfDeath = "";
                }
                this.resource().MetaData.Creators.push(this.creators()[i].creator);
                //c += this.creators()[i].creator.name + "\n";
            }

            //var srces: string = "";
            for (i = 0; i < this.sources().length; i++)
            {
                this.resource().MetaData.Sources.push(this.sources()[i]);
                //srces += this.sources()[i].name + "\n";
            }

            if (!this.validateInput())
                return;

            //alert("uplod pressed " + this.resource().metaData.nameOfPiece
            //       + " " + this.resource().type + "\n"
            //    + this.resource().metaData.dateOfCreation + "\n" + c
            //    + "\n"+ srces
            //);

            alert(JSON.stringify(this.resource()));

            $.ajax(
                {
                    url: "/api/resource/postresource",
                    type: "POST",
                    data: this.resource(),
                    contentType: 'application/json; charset=utf-8',
                    dataType: "json",
                    success: (data) => { alert(data); }
                });

            //fetch("/api/resource/postresource/", {
            //    method: "POST",
            //    mode: "cors",
            //    cache: "no-cache",
            //    headers: {
            //        "Content-Type":"application/json; charset=utf-8"
            //    },
            //    body: JSON.stringify(this.resource()),
            //}).then(data => alert(data.text()));
        };

        this.addCreator = () => {
            this.creators.push({ creator: new CreatorModel.Creator("", "", ""), isDead:new ko.observable(false) });
        }

        this.removeCreator = (creator) =>
        {
            this.creators.remove(creator);
        } 

        this.addSource = () =>
        {
            this.sources.push(new SourceModel.Source("", ""));
        }

        this.removeSource = (src) =>
        {
            this.sources.remove(src);
        }

        this.openFile = (element, event) =>
        {
            var file = event.target.files[0];
            var f = new FileReader();

            f.onload = (e) => {
                var res = f.result;
                //alert("hello" + res.length);

                this.resource().Data = Array.from(new Uint8Array(res));
            };
            f.onerror = (e) => {
                alert("Somthing went wrong whilst reading the file, pleas try again\n" +
                    e);
            };
            f.onprogress = (e) => {
                //TODO progress update
            };
            if (file) {
                f.readAsArrayBuffer(file);
            }

        }

        this.validateInput = () => {

            if (this.resource().MetaData.Creators.length < 1)
            {
                alert("Please enter at least one creator");
                return false;
            }
            if (this.resource().MetaData.Sources.length < 1) {
                alert("Pleas enter at least one source");
                return false;
            }
            if (this.resource().MetaData.License == "")
            {
                alert("Please enter the license under which this resource stands");
                return false;
            }
            if (this.resource().Data == null)
            {
                alert("Please choose a file to upload");
                return false;
            }
            return true;
        };

    }
}

ko.applyBindings(new ResourceViewModel());