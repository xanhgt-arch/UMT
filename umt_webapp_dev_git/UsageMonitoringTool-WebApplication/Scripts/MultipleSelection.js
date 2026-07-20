//Below array variables added for multiple selection enable
var prodCategory = ["FTS", "FBD", "SEALING", "GENERAL"];
var regions = ["EU", "NA", "APAC"];
var harware = ["VDI", "NON-VDI"];
var cadApps = ["CATIA", "NX"];
var domain = ["COOPER", "MERGON", "TATA", "MTC"];
var appNames = [];


//Function to update product Category
//$(document).ready(function () {
//    var select = document.getElementById("AUM_Pc");

//    for (const val of prodCategory) {
//        var option = document.createElement("option");
//        option.value = val;
//        option.text = val.charAt(0).toUpperCase() + val.slice(1);
//        select.appendChild(option);
//    }

//    //var label = document.createElement("label");
//    //label.innerHTML = "Choose your prodcuct Category: "
//    //label.htmlFor = "Cate";

//    //document.getElementById("container").appendChild(label).appendChild(select);
//});

//$(document).ready(function () {
//    var select = document.getElementById("AUM_Region");

//    for (const val of regions) {
//        var option = document.createElement("option");
//        option.value = val;
//        option.text = val.charAt(0).toUpperCase() + val.slice(1);
//        select.appendChild(option);
//    }

//    //var label = document.createElement("label");
//    //label.innerHTML = "Choose your pets: "
//    //label.htmlFor = "pets";

//    //document.getElementById("container").appendChild(label).appendChild(select);
//});

//$(document).ready(function () {
//    var select = document.getElementById("AUM_CadApps");

//    for (const val of cadApps) {
//        var option = document.createElement("option");
//        option.value = val;
//        option.text = val.charAt(0).toUpperCase() + val.slice(1);
//        select.appendChild(option);
//    }

//    //var label = document.createElement("label");
//    //label.innerHTML = "Choose your pets: "
//    //label.htmlFor = "pets";

//    //document.getElementById("container").appendChild(label).appendChild(select);
//});

//$(document).ready(function () {
//    var select = document.getElementById("AUM_Domain");

//    for (const val of domain) {
//        var option = document.createElement("option");
//        option.value = val;
//        option.text = val.charAt(0).toUpperCase() + val.slice(1);
//        select.appendChild(option);
//    }

//    //var label = document.createElement("label");
//    //label.innerHTML = "Choose your pets: "
//    //label.htmlFor = "pets";

//    //document.getElementById("container").appendChild(label).appendChild(select);
//});

//$(document).ready(function () {
//    var select = document.getElementById("AUM_Hardware");

//    for (const val of harware) {
//        var option = document.createElement("option");
//        option.value = val;
//        option.text = val.charAt(0).toUpperCase() + val.slice(1);
//        select.appendChild(option);
//    }

//    //var label = document.createElement("label");
//    //label.innerHTML = "Choose your pets: "
//    //label.htmlFor = "pets";

//    //document.getElementById("container").appendChild(label).appendChild(select);
//});

//AUM_PC
//$(document).ready(function () {
//    $("#AUM_Pc").CreateMultiCheckBox({  defaultText: 'Select Below' });
//});
////AUM_Region
//$(document).ready(function () {
//    $("#AUM_Region").CreateMultiCheckBox({ width: '230px', defaultText: 'Select Below', height: '250px' });
//});
////AUM_CadApps
//$(document).ready(function () {
//    $("#AUM_CadApps").CreateMultiCheckBox({ width: '230px', defaultText: 'Select Below', height: '250px' });
//});
////AUM_DOMAIAN
//$(document).ready(function () {
//    $("#AUM_Domain").CreateMultiCheckBox({ width: '230px', defaultText: 'Select Below', height: '250px' });
//});
////AUM_Hardware
//$(document).ready(function () {
//    $("#AUM_Hardware").CreateMultiCheckBox({ width: '230px', defaultText: 'Select Below', height: '250px' });
//});
////AUM_ApplicationName
//$(document).ready(function () {
//    $("#AUM_App").CreateMultiCheckBox({ width: '230px', defaultText: 'Select Below', height: '250px' });
//});


//$(document).ready(function () {
//    $(document).on("click", ".MultiCheckBox", function () {
//        var detail = $(this).next();
//        detail.show();
//    });

//    $(document).on("click", ".MultiCheckBoxDetailHeader input", function (e) {
//        e.stopPropagation();
//        var hc = $(this).prop("checked");
//        $(this).closest(".MultiCheckBoxDetail").find(".MultiCheckBoxDetailBody input").prop("checked", hc);
//        $(this).closest(".MultiCheckBoxDetail").next().UpdateSelect();
//    });

//    $(document).on("click", ".MultiCheckBoxDetailHeader", function (e) {
//        var inp = $(this).find("input");
//        var chk = inp.prop("checked");
//        inp.prop("checked", !chk);
//        $(this).closest(".MultiCheckBoxDetail").find(".MultiCheckBoxDetailBody input").prop("checked", !chk);
//        $(this).closest(".MultiCheckBoxDetail").next().UpdateSelect();
//    });

//    $(document).on("click", ".MultiCheckBoxDetail .cont input", function (e) {
//        e.stopPropagation();
//        $(this).closest(".MultiCheckBoxDetail").next().UpdateSelect();

//        var val = ($(".MultiCheckBoxDetailBody input:checked").length == $(".MultiCheckBoxDetailBody input").length)
//        $(".MultiCheckBoxDetailHeader input").prop("checked", val);
//    });

//    $(document).on("click", ".MultiCheckBoxDetail .cont", function (e) {
//        var inp = $(this).find("input");
//        var chk = inp.prop("checked");
//        inp.prop("checked", !chk);

//        var multiCheckBoxDetail = $(this).closest(".MultiCheckBoxDetail");
//        var multiCheckBoxDetailBody = $(this).closest(".MultiCheckBoxDetailBody");
//        multiCheckBoxDetail.next().UpdateSelect();

//        var val = ($(".MultiCheckBoxDetailBody input:checked").length == $(".MultiCheckBoxDetailBody input").length)
//        $(".MultiCheckBoxDetailHeader input").prop("checked", val);
//    });

//    $(document).mouseup(function (e) {
//        var container = $(".MultiCheckBoxDetail");
//        if (!container.is(e.target) && container.has(e.target).length === 0) {
//            container.hide();
//        }
//    });
//});

//var defaultMultiCheckBoxOption = { width: '220px', defaultText: 'Select Below', height: '200px' };

//jQuery.fn.extend({
//    CreateMultiCheckBox: function (options) {

//        var localOption = {};
//        localOption.width = (options != null && options.width != null && options.width != undefined) ? options.width : defaultMultiCheckBoxOption.width;
//        localOption.defaultText = (options != null && options.defaultText != null && options.defaultText != undefined) ? options.defaultText : defaultMultiCheckBoxOption.defaultText;
//        localOption.height = (options != null && options.height != null && options.height != undefined) ? options.height : defaultMultiCheckBoxOption.height;

//        this.hide();
//        this.attr("multiple", "multiple");
//        var divSel = $("<div class='MultiCheckBox'>" + localOption.defaultText + "<span class='k-icon k-i-arrow-60-down'><svg aria-hidden='true' focusable='false' data-prefix='fas' data-icon='sort-down' role='img' xmlns='http://www.w3.org/2000/svg' viewBox='0 0 320 512' class='svg-inline--fa fa-sort-down fa-w-10 fa-2x'><path fill='currentColor' d='M41 288h238c21.4 0 32.1 25.9 17 41L177 448c-9.4 9.4-24.6 9.4-33.9 0L24 329c-15.1-15.1-4.4-41 17-41z' class=''></path></svg></span></div>").insertBefore(this);
//        divSel.css({ "width": localOption.width });

//        var detail = $("<div class='MultiCheckBoxDetail'><div class='MultiCheckBoxDetailHeader'><input type='checkbox' class='mulinput' value='-1982' /><div>Select All</div></div><div class='MultiCheckBoxDetailBody'></div></div>").insertAfter(divSel);
//        detail.css({ "width": parseInt(options.width) + 10, "max-height": localOption.height });
//        var multiCheckBoxDetailBody = detail.find(".MultiCheckBoxDetailBody");

//        this.find("option").each(function () {
//            var val = $(this).attr("value");

//            if (val == undefined)
//                val = '';

//            multiCheckBoxDetailBody.append("<div class='cont'><div><input type='checkbox' class='mulinput' value='" + val + "' /></div><div>" + $(this).text() + "</div></div>");
//        });

//        multiCheckBoxDetailBody.css("max-height", (parseInt($(".MultiCheckBoxDetail").css("max-height")) - 28) + "px");
//    },
//    UpdateSelect: function () {
//        var arr = [];

//        this.prev().find(".mulinput:checked").each(function () {
//            arr.push($(this).val());
//        });

//        this.val(arr);
//    },
//});