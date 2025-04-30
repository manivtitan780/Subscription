let dotnetInstance;

function scroll(index) {
    const _grid = document.getElementsByClassName("e-grid")[0].blazor__instance;
    const _rowHeight = 45;
    _grid.getContent().scrollTo({top: (index - 1) * _rowHeight, left: 0, behavior: "smooth"});
}

function alert(message) {
    alert(message);
}

window.changeDivContent = function () {
    let element = document.querySelector('.e-pv-notification-popup-content');
    if (element) {
        element.textContent = "No more matches found.";
    }
}

function detail(dotnet) {
    dotnetInstance = dotnet; // dotnet instance to invoke C# method from JS  
}

document.addEventListener("click", (args) => {
    if (args.target.classList.contains("e-dtdiagonaldown") || args.target.classList.contains("e-detailrowexpand")) {
        dotnetInstance.invokeMethodAsync("DetailCollapse"); // call C# method from javascript function 
    }
});

window.onCreateNoSpecial = (id) => {
    document.getElementById(id).addEventListener("keydown", (e) => {
        let letters = /^[a-zA-Z0-9-_. ]+$/;
        if (e.key.match(letters) || e.key === "Backspace" || e.key === "Delete" || e.key === "ArrowRight" || e.key === "ArrowLeft" || e.key === "Tab" || e.key === "Enter") {
            return true;
        } else {
            e.preventDefault();
        }
    });
};

window.onCreate = (id, numberOnly = false) => {
    document.getElementById(id).addEventListener("keydown", (e) => {
        let letters = numberOnly ? /^[0-9]+$/ : /[a-zA-Z\s\-._\[\]()]/;
        if (e.key.match(letters) || e.key === "Backspace" || e.key === "Delete" || e.key === "ArrowRight" || e.key === "ArrowLeft" || e.key === "Tab" || e.key === "Enter") {
            return true;
        } else {
            e.preventDefault();
        }
    });
};


window.insertTextAtCursor = function (inputID, text) {
    let input = document.getElementById(inputID);
    if (input != null) {
        let _startPos = input.selectionStart;
        let _endPos = input.selectionEnd;
        let _currentValue = input.value;

        let _startText = "", _endText = "";
        if (text == null) {
            text = "";
        }
        _startText = _currentValue.substring(0, _startPos);
        if (_startText == null) {
            _startText = "";
        }
        _endText = _currentValue.substring(_endPos);
        if (_endText == null) {
            _endText = "";
        }
        input.value = _startText + text + _endText;
        input.selectionStart = input.selectionEnd = _startPos + text.length;
        input.focus();
    }
};

window.downloadFileFromBytes = (filename, base64Content) => {
    const link = document.createElement('a');
    link.href = "data:application/octet-stream;base64," + base64Content;
    link.download = filename;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    window.close();
};