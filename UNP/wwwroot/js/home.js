// Модальное окно входа пользователя
if (document.getElementById("loginButton")) {
    document.getElementById("loginButton").addEventListener("click", function () {
        $("#loginModal").modal("show");
    });
}

// Модальное окно регистрации пользователя
if (document.getElementById("registerButton")) {
    document.getElementById("registerButton").addEventListener("click", function () {
        $("#registerModal").modal("show");
    });
}

// Отображение результатов проверки
var invalidUnps = [];
var validUnps = [];

function checkUnps(unpEntryUrl) {
    var checkUnpsButton = document.getElementById("checkUnpsButton");
    if (checkUnpsButton) {
        // Проверяем, добавлен ли обработчик события нажатия кнопки
        if (!checkUnpsButton.hasAttribute("data-clicked")) {
            checkUnpsButton.setAttribute("data-clicked", "true");
            checkUnpsButton.addEventListener("click", function () {
                var unpsInput = document.getElementById("unpsInput").value.trim();
                var unpsArray = unpsInput.split(/[,\s]+/);

                // Очистка массивов при каждом новом вводе пользователя
                invalidUnps = [];
                validUnps = [];

                for (var i = 0; i < unpsArray.length; i++) {
                    var unp = unpsArray[i];

                    if (/^\d+$/.test(unp)) {
                        validUnps.push(unp);
                    } else {
                        invalidUnps.push(unp);
                    }
                }

                if (invalidUnps.length > 0) {
                    alert("Следующие УНП некорректны: " + invalidUnps.join(", "));
                    return;
                }
                $.ajax({
                    url: unpEntryUrl,
                    type: 'POST',
                    data: { Unps: validUnps.join(",") },
                    success: function (data) {
                        var resultsBody = document.getElementById("resultsBody");
                        resultsBody.innerHTML = "";

                        data.forEach(function (item) {
                            var row = "<tr>" +
                                "<td>" + item.unp + "</td>" +
                                "<td>" + item.lastChecked + "</td>" +
                                "<td>" + item.isInLocalDb + "</td>" +
                                "<td>" + item.isInExternalDb + "</td>" +
                                "</tr>";
                            resultsBody.innerHTML += row;
                            var historyTable = document.getElementById("historyTable");
                            var newRow = historyTable.insertRow(1);
                            newRow.innerHTML += row;
                        });
                    },
                    error: function (error) {
                        console.log(error);
                    }
                });
            });
        }
    }
}


// Обработчик клика на строке таблицы
if (document.getElementById("resultsBody")) {
    document.getElementById("resultsBody").addEventListener("click", function (event) {
        var target = event.target;
        while (target && target.parentNode !== this) {
            target = target.parentNode;
            if (!target) { return; }
            if (target.tagName === 'TR') {
                var unp = target.cells[0].innerText;
                openModalWithData(unp);
                return;
            }
        }
    });
}


// Обработчик клика на строке таблицы истории
if (document.getElementById("historyBody")) {
    document.getElementById("historyBody").addEventListener("click", function (event) {
        var target = event.target;
        while (target && target.parentNode !== this) {
            target = target.parentNode;
            if (!target) { return; }
            if (target.tagName === 'TR') {
                var unp = target.cells[0].innerText;
                openModalWithData(unp);
                return;
            }
        }
    });
}

// Функция для открытия модального окна с данными о плательщике
function openModalWithData(unp) {
    var getUnpDetailsUrl = '/Home/GetUnpDetails?unp=' + unp;
    $.ajax({
        url: getUnpDetailsUrl,
        type: 'GET',
        success: function (data) {
            if (data) {
                showModalWithData(data);
            }
        },
        error: function () {
            // Если УНП не найден в локальной базе данных, делаем запрос к внешнему API
            var externalApiUrl = '/Home/GetUnpDetailsFromExternalApi?unp=' + unp;
            $.ajax({
                url: externalApiUrl,
                type: 'GET',
                dataType: 'json',
                success: function (response) {
                    if (response && response.row) {
                        showModalWithData(response.row);
                    }
                },
                error: function () {
                    alert('УНП не найден ни в локальной базе данных, ни во внешней базе.');
                }
            });
        }
    });
}

// Функция для отображения данных в модальном окне
function showModalWithData(data) {
    var modalHtml = `
        <div class="modal fade" id="unpModal" tabindex="-1" role="dialog" aria-labelledby="unpModalLabel" aria-hidden="true">
            <div class="modal-dialog" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="unpModalLabel">Данные о плательщике</h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        <p><strong>УНП:</strong> ${data.vunp || data.Vunp}</p>
                        <p><strong>Полное наименование:</strong> ${data.vnaimp || data.Vnaimp}</p>
                        <p><strong>Краткое наименование:</strong> ${data.vnaimk || data.Vnaimk}</p>
                        <p><strong>Дата постановки на учет:</strong> ${data.dreg || data.Dreg}</p>
                        <p><strong>Код инспекции МНС:</strong> ${data.nmns || data.Nmns}</p>
                        <p><strong>Наименование инспекции МНС:</strong> ${data.vmns || data.Vmns}</p>
                        <p><strong>Код состояния плательщика:</strong> ${data.ckodsost || data.Ckodsost}</p>
                        <p><strong>Дата изменения состояния:</strong> ${data.dlikv || data.Dlikv}</p>
                        <p><strong>Основание изменения состояния:</strong> ${data.vlikv || data.Vlikv}</p>
                        <p><strong>Последняя проверка:</strong> ${data.lastChecked || ''}</p>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-dismiss="modal">Закрыть</button>
                    </div>
                </div>
            </div>
        </div>`;

    $('body').append(modalHtml);
    $('#unpModal').modal('show');

    // Удаляем модальное окно после его закрытия
    $('#unpModal').on('hidden.bs.modal', function () {
        $(this).remove();
    });
}