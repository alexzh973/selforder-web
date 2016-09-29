function showAnalog(goodCode, priceType, ownerid, placeId) {
    setan("Аналоги", "'a'", goodCode, priceType, ownerid, placeId);
}
function showSoput(goodCode, priceType, ownerid, placeId) {
    setan("Сопутствующие", "'so','sz'", goodCode, priceType, ownerid, placeId);
}
function showComplect(goodCode, priceType, ownerid, placeId) {
    setan("Комплектующие", "'k'", goodCode, priceType, ownerid, placeId);
}


function setan(title, antype, goodCode, priceType, ownerid, placeId) {
    $.get("../good/gdetail.ashx",
        {
            sid: sid,
            act: "angood",
            code: goodCode,
            antype: antype,
            typecen: priceType,
            own: ownerid
        },
         function (data) {
             if (data && data.length > 10) {
                 $("#" + placeId).show();
                 $("#" + placeId).html("<h5>" + title + "</h5>" + data + "");

             } else {
                 $("#" + placeId).html("");
                 $("#" + placeId).hide();
             }
         }
    );
}
