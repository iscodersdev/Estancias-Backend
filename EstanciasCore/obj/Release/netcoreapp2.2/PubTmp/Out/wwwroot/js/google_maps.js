//Muestra en el mapa la direccion seleccionada
function crearMapa(latitud_cliente, longitud_cliente, mapa, title) {
    $('#div_calle').show();
    lat_val = parseFloat(latitud_cliente);
    lng_val = parseFloat(longitud_cliente);
    var mapa = document.getElementsByClassName(mapa)[0];
    map = new google.maps.Map(mapa, {
        center: { lat: lat_val, lng: lng_val },
        zoom: 15
    });
    var marker = new google.maps.Marker({
        position: { lat: lat_val, lng: lng_val },
        map: map,
        title: title
    });
    $('#mapa_manual').prop("checked", false);
    $(mapa).show();
}

//Va mostrando las posibles direcciones cada vez que el usuario ingresa una letra en el buscador
function initializeMultiInputs() {
    var acInputs = document.getElementsByClassName("autocomplete");
    for (var i = 0; i < acInputs.length; i++) {
        var autocomplete = new google.maps.places.Autocomplete(acInputs[i]);
        autocomplete.inputId = acInputs[i].id;
        google.maps.event.addListener(autocomplete, 'place_changed', function () {
            place = this.getPlace();

            var input = $("#" + this.inputId);
            var input_latitud = $("input[name='" + input.data("latitud") + "']");
            var input_longitud = $("input[name='" + input.data("longitud") + "']");

            $(input_latitud).val(place.geometry.location.lat());
            $(input_longitud).val(place.geometry.location.lng());
            for (var i = 0; i < place.address_components.length; i++) {
                var addressType = place.address_components[i].types[0];
                var value = place.address_components[i].short_name;

                if (addressType == "postal_code") {
                    $("input[name='" + $(input).data("codigo-postal") + "']").val(value);
                }
                if (addressType == "street_number") {
                    $("input[name='" + $(input).data("altura") + "']").val(value);
                    $("input[name='" + $(input).data("altura") + "']").attr('readonly', 'readonly');
                }
                if (addressType == "sublocality_level_1") {
                    $("input[name='" + $(input).data("barrio") + "']").val(value);
                }
                if (addressType == "route") {
                    $("input[name='" + $(input).data("calle") + "']").val(value);
                    $("input[name='" + $(input).data("calle") + "']").attr('readonly', 'readonly');
                }
                if (addressType == "locality") {
                    $("input[name='" + $(input).data("localidad") + "']").val(value);
                    $("input[name='" + $(input).data("localidad") + "']").attr('readonly', 'readonly');
                }
                if (addressType == "administrative_area_level_1") {
                    pos_borrar = value.toLowerCase().indexOf("provincia de ");
                    if (pos_borrar != -1) {
                        value = value.substring(13);
                    }
                    $("input[name='" + $(input).data("provincia") + "']").val(value);
                    $("input[name='" + $(input).data("provincia") + "']").attr('readonly', 'readonly');
                }
                if (addressType == "administrative_area_level_2") {
                    pos_borrar = value.toLowerCase().indexOf("Ciudad de ");
                    if (pos_borrar != -1) {
                        value = value.substring(13);
                    }
                    if (value != "")
                        $("input[name='" + $(input).data("localidad") + "']").val(value);
                        $("input[name='" + $(input).data("localidad") + "']").attr('readonly', 'readonly');
                }
                if (addressType == "country") {
                    if (value != 'AR') {
                        $('#Pais').attr('readonly', 'readonly');
                        $("#Domicilio_LocalidadId").empty();
                        $("#Domicilio_ProvinciaId").empty();
                    }
                    $("input[name='" + $(input).data("pais") + "']").val(place.address_components[i].long_name);
                }
            }
            crearMapa($(input_latitud).val(), $(input_longitud).val(), input.data("mapa"));
            getData();
        });
    }
    function getData() {

        var id;
        var nombre;
        $("#Domicilio_ProvinciaId").empty();
        $('#Domicilio_LocalidadId').empty();        
        $.ajax({
            type: "GET",
            url: 'https://apis.datos.gob.ar/georef/api/ubicacion?lat=' + $("#latitud").val() + '&lon=' + $("#longitud").val(),
            async: false,
            dataType: "json",
            success: function (data) {
                if (data['ubicacion']['provincia']['id']) {
                    id = data['ubicacion']['provincia']['id'];
                    nombre = data['ubicacion']['provincia']['nombre'];
                    $("#Domicilio_ProvinciaId").append('<option value=' + id + '>' + nombre + '</option>');
                    $("#Domicilio_ProvinciaId").selectpicker('refresh');
                }
                if (data['ubicacion']['departamento']['id']) {
                    id = data['ubicacion']['departamento']['id'];
                    nombre = data['ubicacion']['departamento']['nombre'];
                    $.ajax({
                        type: "GET",
                        url: '/Localidades/GetLocalidades/' + id,
                        async: false,
                        dataType: "json",
                        success: function (data) {
                            var id2;
                            var nombre2;
                            $("#Domicilio_LocalidadId").empty();
                            $("#Domicilio_LocalidadId").append('<option value="">Seleccione </option>');
                            $.each(data, function (key, registro) {
                                $.each(registro, function (key, value) {
                                    if (key == 'Value') { id2 = value }
                                    if (key == 'Text') { nombre2 = value }
                                    if (key == 'Selected') { seleccionado = value }
                                })
                                $("#Domicilio_LocalidadId").append('<option value=' + id2 + '>' + nombre2 + '</option>');
                            });
                        },
                        error: function (data) {
                            alert('error');
                        }
                    });
                    $("#Domicilio_ProvinciaId").selectpicker('refresh');
                    $("#Domicilio_LocalidadId").selectpicker('refresh');
                }
            },
            error: function (data) {
                alert('error');
            }
        });
    }
} 