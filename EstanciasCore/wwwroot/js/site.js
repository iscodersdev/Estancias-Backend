// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {
    /////********//// Funcion Captura cualquier select cuando cambia /////********////
    $('#CantHermanos').change(function () {
        var rel = getRelVF($('#CantHermanos').val(), $('#CantHermanosVivos').val())
        $('#CantHermanosFallecidos').val(rel);
    })
    $('#CantHermanosVivos').change(function () {
        var rel = getRelVF($('#CantHermanos').val(), $('#CantHermanosVivos').val())
        $('#CantHermanosFallecidos').val(rel);
    })
    $('#CantHijos').change(function () {
        var rel = getRelVF($('#CantHijos').val(), $('#CantHijosVivos').val())
        $('#CantHijosFallecidos').val(rel);
    })
    $('#CantHijosVivos').change(function () {
        var rel = getRelVF($('#CantHijos').val(), $('#CantHijosVivos').val())
        $('#CantHijosFallecidos').val(rel);
    })
    function getRelVF($total, $vivos) {
        var t = $total,
            v = $vivos,
            rel = t - v;
        return rel;
    };
    $("#Financiador_Id").change(function () {
        LoadPlanes();
    });

    function LoadPlanes() {
        var financiadorId = "";
        financiadorId += $("#Financiador_Id option:selected").val();
        $.ajax({
            url: "/FinanciadoresPlanes/JsonPlanes?=" + financiadorId,
            type: "POST",
            dataType: "JSON",
            success: function (planes) {
                if ($("#Financiador_Id").val() != null) {
                    $("#FinanciadoresPlanes_Id").prop('disabled', false)
                    $('button[data-id="FinanciadoresPlanes_Id"]').removeClass('disabled')
                } else {
                    $("#FinanciadoresPlanes_Id").prop('disabled', true)
                    $('button[data-id="FinanciadoresPlanes_Id"]').addClass('disabled')
                }
                $('button[data-id="FinanciadoresPlanes_Id"]').find('span.filter-option').html("Seleccione")
                $('#FinanciadoresPlanes_Id').val(0)
                $('#FinanciadoresPlanes_Id').empty()
                $('#planes').find('ul').empty()
                $('#planes').find('ul').append('<li data-original-index="0"><a tabindex="0" class="" data-normalized-text="<span class=&quot;text&quot;>Seleccione</span>"><span class="text">Seleccione</span><span class="glyphicon glyphicon-ok check-mark"></span></a></li>');
                $('#FinanciadoresPlanes_Id').append("<option value='0'>Seleccione</option>");
                $.each(planes, function (planes) {
                    $('#FinanciadoresPlanes_Id').append("<option value='" + this.Value + "'>" + this.Text + "</option>");
                    $('#planes').find('ul').append('<li data-original-index="' + this.Value + '"><a tabindex="0" class="" data-normalized-text="<span class=&quot;text&quot;>' + this.Text + '</span>"><span class="text">' + this.Text + '</span><span class="glyphicon glyphicon-ok check-mark"></span></a></li>');
                });
                $('#FinanciadoresPlanes_Id').selectpicker('refresh');
            }
        });
    }
    $(".form-alert").submit(function (e) {
        e.preventDefault();

        Swal.fire({
            title: "Confirmación!",
            text: "¿Esta seguro de insertar este registro?",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: "Si, estoy seguro!",
        }).then((result) => {
            if (result.isConfirmed) {
                this.submit();
            }
        })

    });
    //$("#FinanciadorPlanes_Id").change(function () {
    //    LoadFinanciadores();
    //});

    //function LoadFinanciador() {
    //    var planId = "";
    //    planId += $("#Financiador_Id option:selected").val();
    //    $.ajax({
    //        url: "/FinanciadoresPlanes/JsonPlanes?=" + planId,
    //        type: "POST",
    //        dataType: "JSON",
    //        success: function (planes) {
    //            if ($("#FinanciadoresPlanes_Id").val() != null) {
    //                $("#Financiador_Id").prop('disabled', false)
    //                $('button[data-id="Financiador_Id"]').removeClass('disabled')
    //            } else {
    //                $("#Financiador_Id").prop('disabled', true)
    //                $('button[data-id="Financiador_Id"]').addClass('disabled')
    //            }
    //            $('button[data-id="Financiador_Id"]').find('span.filter-option').html("Seleccione")
    //            $('#Financiador_Id').val(0)
    //            $('#Financiador_Id').empty()
    //            $('#financiador').find('ul').empty()
    //            $('#financiador').find('ul').append('<li data-original-index="0"><a tabindex="0" class="" data-normalized-text="<span class=&quot;text&quot;>Seleccione</span>"><span class="text">Seleccione</span><span class="glyphicon glyphicon-ok check-mark"></span></a></li>');
    //            $('#Financiador_Id').append("<option value='0'>Seleccione</option>");
    //            $.each(planes, function (planes) {
    //                $('#Financiador_Id').append("<option value='" + this.Value + "'>" + this.Text + "</option>");
    //                $('#planes').find('ul').append('<li data-original-index="' + this.Value + '"><a tabindex="0" class="" data-normalized-text="<span class=&quot;text&quot;>' + this.Text + '</span>"><span class="text">' + this.Text + '</span><span class="glyphicon glyphicon-ok check-mark"></span></a></li>');
    //            });
    //            $('#Financiador_Id').selectpicker('refresh');
    //        }
    //    });
    //}
});
///////////////////////////////////////////
/////**********  ON CHECK ***********//////
//////////////////////////////////////////
//$('#DatoEscalafonario_EnActividad').click(function () {
//    if ($(this).is(':checked')) {
//        $(this).val('true');
//        $('.Activo').show();
//    } else {
//        $('.Activo').hide();
//        $("#DatoEscalafonario_ArmaId").val("0");
//        $("#DatoEscalafonario_UnidadId").val("0");
//        $("#DatoEscalafonario_ArmaId").selectpicker('refresh');
//        $("#DatoEscalafonario_UnidadId").selectpicker('refresh');
//    }
//});
//function mostrar(idchecked, objamostrar) {
//    if ($(idchecked).is(':checked'))
//        $(objamostrar).show();
//    else
//        $(objamostrar).hide();
//}
//mostrar('#DatoEscalafonario_EnActividad', '.Activo');
$('#Cancelada').click(function () {
    if ($(this).is(':checked')) {
        $(this).val('true');
        $('.MotivoCancelacion').show();
    } else {
        $('.MotivoCancelacion').hide();
        $("#MotivoCancelacion").val("");
    }
});
//function mostrar(idchecked, objamostrar) {
//    if ($(idchecked).is(':checked'))
//        $(objamostrar).show();
//    else
//        $(objamostrar).hide();
//}
//mostrar('#Cancelada', '.MotivoCancelacion');