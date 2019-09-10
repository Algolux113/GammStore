"use strict";

(function () {
    var buyGameBtns = document.getElementsByClassName("buyGameBtn");

    Array.prototype.forEach.call(buyGameBtns, function (buyGameBtn) {
        buyGameBtn.addEventListener("click", function () {
            var gameId = buyGameBtn.dataset.gameId;
            var xhr = new XMLHttpRequest();

            xhr.open("GET", "Basket/AddBasket?gameId=" + gameId, true);

            xhr.onreadystatechange = function () {
                if (xhr.readyState !== 4) {
                    return;
                }

                var data = JSON.parse(this.response);
                var message = data.message;

                if (data.statusCode === 200) {
                    document.getElementById("basketCnt").innerHTML = data.basketCnt;
                }

                buyGameBtn.dataset.content = message;
                $(buyGameBtn).popover('show');
            };

            xhr.send();
        }, false);
    });

    var deleteGameBtns = document.getElementsByClassName("deleteGameBtn");

    Array.prototype.forEach.call(deleteGameBtns, function (deleteGameBtn) {
        deleteGameBtn.addEventListener("click", function () {
            var gameId = deleteGameBtn.dataset.gameId;
            var xhr = new XMLHttpRequest();

            xhr.open("GET", "Basket/DeleteBasket?gameId=" + gameId, true);

            xhr.onreadystatechange = function () {
                if (xhr.readyState !== 4) {
                    return;
                }

                var data = JSON.parse(this.response);

                if (data.statusCode === 200) {
                    document.getElementById("basketCnt").innerHTML = data.basketCnt;

                    if (data.basketCnt !== 0) {
                        document.getElementById("basketTotalSum").innerHTML = data.basketTotalSum
                            .toFixed(2)
                            .toString().replace('.', ',');

                        deleteGameBtn.closest("tr").remove();
                    }
                    else {
                        document.getElementById("basketsContent").innerHTML = "<p>Корзина пуста.</p>";
                    }
                }
            };

            xhr.send();
        }, false);
    });

    var basketQuantityInputs = document.getElementsByClassName("basketQuantityInput");

    Array.prototype.forEach.call(basketQuantityInputs, function (basketQuantityInput) {
        basketQuantityInput.addEventListener("change", function () {
            if (!/^[0-9]+$/.test(basketQuantityInput.value) || !basketQuantityInput.valueAsNumber) {
                basketQuantityInput.value = basketQuantityInput.defaultValue;
            }

            var gameId = basketQuantityInput.dataset.gameId;

            var basket = {
                GameId: gameId,
                Quantity: basketQuantityInput.value
            };

            var xhr = new XMLHttpRequest();

            xhr.open("POST", "Basket/ChangeBasket", true);
            xhr.setRequestHeader("Content-type", "application/json");

            xhr.onreadystatechange = function () {
                if (xhr.readyState !== 4) {
                    return;
                }

                var data = JSON.parse(this.response);

                if (data.statusCode === 200) {
                    document.getElementById("basketCnt").innerHTML = data.basketCnt;
                    document.getElementById("basketTotalSum").innerHTML = data.basketTotalSum.toFixed(2)
                        .toString().replace('.', ',');

                    basketQuantityInput
                        .closest("tr")
                        .getElementsByClassName("basketSum")[0]
                        .innerHTML = data.basketSum.toFixed(2)
                            .toString().replace('.', ',');
                }
            };

            xhr.send(JSON.stringify(basket));
        }, false);
    });

    var pushOrderBtn = document.getElementById("pushOrderBtn");

    if (pushOrderBtn !== null) {
        pushOrderBtn.addEventListener("click", function () {
            var xhr = new XMLHttpRequest();

            xhr.open("GET", "Basket/PushOrder", true);

            xhr.onreadystatechange = function () {
                if (xhr.readyState !== 4) {
                    return;
                }

                var data = JSON.parse(this.response);

                if (data.statusCode === 200) {
                    document.getElementById("basketCnt").innerHTML = data.basketCnt;
                    document.getElementById("basketsContent").innerHTML =
                        "<p>Заказ успешно оформлен.</p>" +
                        "<p><a class='btn btn-secondary' href='/Order/Index'" +
                        "role='button'>Заказы</a></p>";
                }
                else if (data.statusCode === 500) {
                    console.log(data.message);
                }
            };

            xhr.send();
        });
    }
})();
