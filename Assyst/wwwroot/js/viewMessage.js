function createViewMessage(elementId, name, type, text, title) {
    $.ajax({
        type: 'POST',
        url: "/HtmlHelper/ViewMessagePartialView/",
        data: {
            name: name,
            type: type,
            text: text,
            title: title
        },
        dataType: "html",
        success: function (data) {
            $(elementId).html(data);
        },
        error: function (data) {

        }
    });
}

var ViewMessage = (function () {

    // Constructor
    function ViewMessage(name, type, text, title) {
        this.name = name;
        this.type = type;
        this.text = text;
        this.title = title;
        this.control = document.getElementById(this.name);
    }

    ViewMessage.prototype.show = function (type, text, title) {
        text = clearDelimer(text, "***");
        $("#" + this.name).removeClass("^='alert-'");
        $("#" + this.name).addClass("alert-" + type);
        $("#" + this.name).find("p").html(text);
        $("#" + this.name).find("h4").html(title);
        $("#" + this.name).addClass("show");
        $("#" + this.name).removeClass("d-none");
        $(window).trigger('resize');
    };

    ViewMessage.prototype.hide = function () {
        $("#" + this.name).addClass("d-none");
        $("#" + this.name).removeClass("^='alert-'");
        $("#" + this.name).removeClass("show");
        $("#" + this.name).find("p").html("");
        $("#" + this.name).find("h4").html("");
        $(window).trigger('resize');
    };

    return ViewMessage;
})();









