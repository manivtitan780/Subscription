function scroll(index) {
    const _grid = document.getElementsByClassName("e-grid")[0].blazor__instance;
    const _rowHeight = 45;
    _grid.getContent().scrollTo({ top: (index - 1) * _rowHeight, left: 0, behavior: "smooth" });
}

function alert(message) {
    alert(message);
}

var dotnetInstance;

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
        var letters = /^[a-zA-Z0-9-_. ]+$/;
        if (e.key.match(letters) || e.key == "Backspace" || e.key == "Delete" || e.key == "ArrowRight" || e.key == "ArrowLeft" || e.key == "Tab" || e.key == "Enter") {
            return true;
        }
        else {
            e.preventDefault();
        }
    });
};

window.onCreate = (id, numberOnly = false) => {
    document.getElementById(id).addEventListener("keydown", (e) => {
        var letters = numberOnly ? /^[0-9]+$/ : /[a-zA-Z\s\-\._\[\]\(\)]/;
        if (e.key.match(letters) || e.key == "Backspace" || e.key == "Delete" || e.key == "ArrowRight" || e.key == "ArrowLeft" || e.key == "Tab" || e.key == "Enter") {
            return true;
        }
        else {
            e.preventDefault();
        }
    });
};


window.insertTextAtCursor = function (inputID, text) {
    var input = document.getElementById(inputID);
    if (input != null) {
        var _startPos = input.selectionStart;
        var _endPos = input.selectionEnd;
        var _currentValue = input.value;

        var _startText = "", _endText = "";
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