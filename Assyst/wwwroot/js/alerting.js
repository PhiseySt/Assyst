var popups = (
    {
        // информация о серверных ошибках, возвращаемых с backend
        errormessage: function (data) {
            var delimiter = "***";
            var lengthDelimiter = delimiter.length;
            var htmlText = data.responseText;
            var firstPosDelimiter = htmlText.indexOf(delimiter);
            var secondPosDelimiter = htmlText.indexOf(delimiter, firstPosDelimiter + 1);
            var lengthErrorText = secondPosDelimiter - firstPosDelimiter - lengthDelimiter;
            var errorText = htmlText.substr(firstPosDelimiter + lengthDelimiter, lengthErrorText);

            var elem = document.createElement('textarea');
            elem.innerHTML = errorText;
            var decoded = elem.value;

            var span = document.createElement("span");
            span.innerHTML = decoded;
            if (lengthErrorText > 0)
                swal({
                    title: "Ошибка",
                    content: span
                });
        },
        // информационные сообщения с frontend
        infomessage: function(alertText) {
            swal({
                title: "Информация",
                text: alertText
            });
        },
        question: function (alertText, f) {
            swal(alertText, {
                buttons: {
                    catch: {
                        text: "Да",
                        value: "catch"
                    },
                    cancel: "Нет"
                    
                }
            })
                .then((value) => {
                    switch (value) {
                        case "catch":
                            f();
                        break;
                    }
                });
        },
        saveChanged: function (message, f1, f2) {
            swal(message, {
                buttons: {
                    defeat: {
                        text: "Сохранить",
                        value: "defeat"
                    },
                    catch: {
                        text: "Не сохранять",
                        value: "catch"
                    },
                    cancel: "Отмена"
                }
            })
                .then((value) => {
                    switch (value) {
                        case "defeat":
                            f1();
                            break;
                        case "catch":
                            f2();
                            break;
                    }
                });
        }
    }
)