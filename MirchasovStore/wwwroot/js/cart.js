var IsInCartPage = false;
var MustReloadCompletely = false;
var UpdatedId = null;

function CartRequest(meth, wareid, qty) {
    $.ajax({
        //url: UserAddressPrefix + "/" + meth + "/" + wareid + "?qty=" + qty,
        url: meth + "/" + wareid + "?qty=" + qty,
        method: "POST",
        data: { isInCart: IsInCartPage },
        success: function (Data, Status, jqXHR) {
            CartUpdateComplete(Data);
        }
    });
}


function EmptyCart(ev) {
    CartRequest("emptyCart", "0", "0");
}

function CartUpdateComplete(Data) {
    if (Data.newCart !== null) {
        $("#mmxCartTableContainer").html(Data.newCart);
        //$("input[data-wareid]").change(CartQtyChanged);
        //$("[data-change]").click(CartQtyClick);
    }

    var allWared = $("[data-wareid]");
    allWared.filter("input").val("");
    allWared.filter("[data-mode=rcont]").hide();
    for (var i = 0; i < Data.items.length; i++) {
        var x = Data.items[i];
        var allRelated = allWared.filter("[data-wareid=" + x.wareid + "]");
        var inputs = allRelated.filter("input"); inputs.val(x.purchased);
        var requiredCont = allRelated.filter("[data-mode=rcont]");
        if (x.required === "") {
            requiredCont.hide();
        } else {
            requiredCont.find("[data-mode=rval]").text(x.required);
            requiredCont.show();
        }
    }

    if (UpdatedId !== null) {
        var buttonBuy = $("#purchaseBtn");
        var miniCartArea = $('#topLine_CartContainer');

        buttonBuy.addClass('buyComplete');
        buttonBuy.text('Добавлено');
        miniCartArea.addClass('miniCartBuyComplete');

        setTimeout(function () { buttonBuy.removeClass('buyComplete'); }, 3000);
        setTimeout(function () { buttonBuy.text('Добавить в корзину'); }, 3000);
        setTimeout(function () { miniCartArea.removeClass('miniCartBuyComplete'); }, 3000);

        UpdatedId = null;
    }

    if (Data.items.length) {
        $("#topLine_CartContainer").addClass("topLine__item_active");
        $("#flyCartBtn").addClass("active");
        $(".floatOrder__outer").addClass("floatOrder__outer_open");
        $(".cartCounter").addClass("cartCounter_active");
        $(".miniCart2__counter").addClass("miniCart2__counter_active");
        $(".mmxCartUnits").html(Data.unitCount); // .cartBarUnits + #cartCellUnits
        $(".mmxCartArts").html(Data.artCount); // #cartCellArts + unused #cartBarArts
        $(".mmxCartTotalSum").html(Data.totalSum); // #cartCellSum + unused #cartBarTotal
        $(".mmxCartIsEmpty").hide(); // #cartCellEmpty
        $("#mmxCartTableContainer").show(); // #cartCellData
        $(".mmxCartTotals").show(); // #cartCellTotals
        $(".mmxCartConfirm").show(); // #cardCellConfirm
        $(".mmxCartIsEmptyIcon").hide(); // #cartIconEmpty
        $(".mmxCartIsNotEmptyIcon").show(); // #cartIconNotEmpty
        if (IsInCartPage) {
            EnableDelivery(Data.eligibleForDelivery);
        }
    } else {
        $("#topLine_CartContainer").removeClass("topLine__item_active");
        $("#flyCartBtn").removeClass("active");
        $(".floatOrder__outer").removeClass("floatOrder__outer_open");
        $(".cartCounter").removeClass("cartCounter_active");
        $(".miniCart2__counter").removeClass("miniCart2__counter_active");
        $(".mmxCartUnits").html(""); // .cartBarUnits
        $(".mmxCartArts").html("");
        $(".mmxCartIsEmpty").show(); // #cartCellEmpty
        $("#mmxCartTableContainer").hide(); // #cartCellData
        $(".mmxCartTotals").hide(); // #cartCellTotals
        $(".mmxCartConfirm").hide(); // #cardCellConfirm
        $(".mmxCartIsEmptyIcon").show(); // #cartIconEmpty
        $(".mmxCartIsNotEmptyIcon").hide(); // #cartIconNotEmpty
    }

    if (Data.discountDesc !== null) {
        $(".mmxDiscountAbsent").hide();
        $(".mmxDiscountPresent").show();
        $(".mmxDiscountDescription").html(Data.discountDesc).show();
    } else {
        $(".mmxDiscountAbsent").show();
        $(".mmxDiscountPresent").hide();
    }
}

function addToCart(ev) {
    var $this = $(ev.target).closest("[data-wareid][data-change]"); //currentTarget
    if (!$this.length) return;
    var wareid = $this.data("wareid");
    UpdatedId = wareid;
    var delta = $this.data("change");
    if (delta === "0") {
        var v = $this.parents("[data-mode=qcont]").find("input").val();
        CartRequest("set", wareid, v);
    } else {
        CartRequest("change", wareid, delta);
    }
}


$(function () {
    $(".layout")
        .click(addToCart);
    $(".mmxEmptyCart").click(EmptyCart);

})