
function LoadImage() {
    let canvas = document.getElementById("myCanvas");
    let context = canvas.getContext('2d');

    let image = new Image();
    image.src= './ImageTest.png';

    image.onload = function() {
        canvas.width = image.width;
        canvas.height = image.height;
        context.drawImage(image, 0, 0);
    }
}

async function LoadBigJSON() {
    const response = await fetch('./Data.json');
    const json = await response.json();
    return json.responses[0].textAnnotations;
}

function ExtractBigHeaders (json){
    return ['Description'];
}

async function LoadJSON() {
    const response = await fetch('./Json.json');
    const json = await response.json();
    return json;
}

function ExtractHeaders (json){
    let result = [];
    for (let i = 0; i < json.length; i++) {
        for (let key in json[i]){
            if (result.indexOf(key) === -1) {
                result.push(key);
            }
        }
    }
    return result;
}

async function FillTable2(){
    const json = await LoadJSON();
    let headers = ExtractHeaders(json);

    let myTable = document.getElementById("myTable2");

    // Apprend headers.
    for (let i = 0; i < headers.length; i++) {
        let th = document.createElement("th");
        if (headers[i] === "Locations")
            continue;
        th.innerHTML = headers[i];
        myTable.appendChild(th);
    }

    // Append rows.
    for (let i = 0; i < json.length; i++) {
        let tr = myTable.insertRow(-1);

        tr.addEventListener('mouseover', function() {
            let canvas = document.getElementById("myCanvas");
            let context = canvas.getContext('2d');

            let vertices = json[i].Locations;

            context.moveTo(vertices[0].x, vertices[0].y);
            context.lineTo(vertices[1].x, vertices[1].y);
            context.lineTo(vertices[2].x, vertices[2].y);
            context.lineTo(vertices[3].x, vertices[3].y);
            context.lineTo(vertices[0].x, vertices[0].y);
            context.strokeStyle = '#ff0000';
            context.stroke();
        });

        tr.addEventListener('mouseleave', function() {
            LoadImage();
        });

        let nbFilledUpColumn = 0;
        for (let j = 0; j < headers.length - 1; j++) {
            let cell = tr.insertCell(-1);
            cell.innerHTML = json[i][headers[[j]]];
            
            if(json[i][headers[[j]]] != undefined && json[i][headers[[j]]] != null && json[i][headers[[j]]] != "" && json[i][headers[[j]]] != "?"){
                console.log(json[i][headers[[j]]]);
                nbFilledUpColumn++;
            }
        }
        
        if(nbFilledUpColumn <= 1)
            tr.style.backgroundColor = "#CC0000";
        else if(nbFilledUpColumn < 3)
            tr.style.backgroundColor = "#AA9900";
        else
            tr.style.backgroundColor = "#007700";
    }
}

async function FillTable(){
    const json = await LoadBigJSON();
    json.shift();
    let headers = ExtractBigHeaders(json);

    let myTable = document.getElementById("myTable");

    // Apprend headers.
    for (let i = 0; i < headers.length; i++) {
        let th = document.createElement("th");
        if (headers[i] === "boundingPoly")
            continue;
        th.innerHTML = headers[i];
        myTable.appendChild(th);
    }

    // Append rows.
    for (let i = 0; i < json.length; i++) {
        let tr = myTable.insertRow(-1);

        tr.addEventListener('mouseover', function() {
            let canvas = document.getElementById("myCanvas");
            let context = canvas.getContext('2d');

            let vertices = json[i].boundingPoly.vertices;

            context.moveTo(vertices[0].x, vertices[0].y);
            context.lineTo(vertices[1].x, vertices[1].y);
            context.lineTo(vertices[2].x, vertices[2].y);
            context.lineTo(vertices[3].x, vertices[3].y);
            context.lineTo(vertices[0].x, vertices[0].y);
            context.strokeStyle = '#ff0000';
            context.stroke();
        });

        tr.addEventListener('mouseleave', function() {
            LoadImage();
        });

        for (let j = 0; j < headers.length; j++) {
            let cell = tr.insertCell(-1);
            cell.innerHTML = json[i].description;
        }
    }
}

function DocumentLoaded() {
    LoadImage();
    FillTable();
    FillTable2();
}