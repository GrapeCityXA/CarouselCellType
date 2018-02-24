var CarouselCellType = (function (_super) {
    __extends(CarouselCellType, _super);
    function CarouselCellType() {
        return _super !== null && _super.apply(this, arguments) || this;
    }

    CarouselCellType.prototype.foreColor = "#FFF";
    CarouselCellType.prototype.hasLoadImages = false;
    CarouselCellType.prototype.createContent = function () {
        var element = this.CellElement;
        var cellTypeMetaData = element.CellType;

        var carouselDiv = $("<div id=" + this.ID + "></div>");
        carouselDiv.addClass("carousel slide");
        carouselDiv.attr("data-ride", "carousel");
        carouselDiv.css("width", "100%");
        carouselDiv.css("height", "100%");

        if (element.StyleInfo.Foreground && element.StyleInfo.Foreground !== "") {
            this.foreColor = element.StyleInfo.Foreground;
        }
        if (cellTypeMetaData.ShowIndicators) {
            this.addCssStyle("#" + this.ID + " .carousel-indicators .active { background-color: " + this.foreColor + ";}");
        }

        if (cellTypeMetaData.IsBinding) {
            this.getImageInfosFromServer(cellTypeMetaData);

            var self = this;
            this.onDependenceCellValueChanged(function (uiAction) {
                $("#" + self.ID).empty();
                self.hasLoadImages = false;
                self.getImageInfosFromServer(cellTypeMetaData);
                self.addIndicatorsEvent();
            });
        }
        
        return carouselDiv;
    }

    CarouselCellType.prototype.addIndicatorsEvent = function () {
        $('#' + this.ID).on('slid.bs.carousel', function () {
            $holder = $("ol li.active");
            $holder.removeClass('active');
            var idx = $('div.active').index('div.item');
            $('ol.carousel-indicators li[data-slide-to="' + idx + '"]').addClass('active');
        });
        $('ol.carousel-indicators li').on("click", function () {
            $('ol.carousel-indicators li.active').removeClass("active");
            $(this).addClass("active");
        });
    }

    CarouselCellType.prototype.initCarouselDiv = function (imageInfos, cellTypeMetaData) {
        if (this.hasLoadImages) {
            return;
        }

        if (imageInfos == null || imageInfos.length <= 0) {
            return;
        }

        var carouselDiv = $("#" + this.ID);
        //<!-- Indicators -->
        if (cellTypeMetaData.ShowIndicators) {
            var indicators = this.getIndicators(imageInfos.length);
            carouselDiv.append(indicators);
        }

        //<!-- Wrapper for slides -->
        var wrapper = this.getWrapperForSlides(imageInfos, cellTypeMetaData.ShowCaptions, cellTypeMetaData.IsBinding);
        carouselDiv.append(wrapper);

        //<!-- Left and right controls -->
        if (cellTypeMetaData.ShowLeftRightControls) {
            carouselDiv.append(this.getLeftRightControls("left"));
            carouselDiv.append(this.getLeftRightControls("right"));
        }
    }

    CarouselCellType.prototype.getImageInfosFromServer = function (cellTypeMetaData) {
        var bindingImageInfo = cellTypeMetaData.BindingImageInfo;
        if (!bindingImageInfo) {
            return;
        }

        var tableName = bindingImageInfo.TableName;
        var columns = this.getBindingColumnNames(bindingImageInfo);
        if (tableName == null || tableName == "" || columns === null) {
            return;
        }

        var param = {
            TableName: tableName,
            Columns: columns,
            QueryCondition: bindingImageInfo.QueryCondition,
            QueryPolicy: {
                Distinct: true,
                QueryNullPolicy: Forguncy.QueryNullPolicy.QueryAllItemsWhenValueIsNull,
                IgnoreCache: false
            }
        };
        var self = this;
        Forguncy.getTableDataByCondition(param, { IsInMasterPage: this.IsInMasterPage }, function (dataStr) {
            if (dataStr != null) {
                var data = JSON.parse(dataStr);
                var items = [];
                for (var i = 0; i < data.length; i++) {
                    var itemInfo = {};
                    itemInfo.Caption = data[i][bindingImageInfo.CaptionColumn];
                    itemInfo.Description = data[i][bindingImageInfo.DescriptionColumn];
                    itemInfo.ImagePath = data[i][bindingImageInfo.ImageColumn];

                    items.push(itemInfo);
                }
                self.initCarouselDiv(items, cellTypeMetaData);
                self.hasLoadImages = true;
                self.startCarousel();
            }
        }, true);
    }

    CarouselCellType.prototype.getBindingColumnNames = function (bindingImageInfo) {
        var captionColumn = bindingImageInfo.CaptionColumn;
        var descriptionColumn = bindingImageInfo.DescriptionColumn;
        var imageColumn = bindingImageInfo.ImageColumn;

        var columnsArray = [];
        if (captionColumn != null && captionColumn != "") {
            columnsArray.push(captionColumn);
        }
        if (descriptionColumn != null && descriptionColumn != "" && descriptionColumn != captionColumn) {
            columnsArray.push(descriptionColumn);
        }

        columnsArray.push(imageColumn);

        return columnsArray;
    }

    CarouselCellType.prototype.addCssStyle = function (rule) {
        var stylesheet = null;
        for (var i in document.styleSheets) {
            if (document.styleSheets[i].href && document.styleSheets[i].href.indexOf("forguncyPluginBootstrap.min.css") !== -1) {
                stylesheet = document.styleSheets[i];
                break;
            }
        }
        if (stylesheet != null) {
            stylesheet.insertRule(rule, stylesheet.cssRules.length);
        }
    }

    CarouselCellType.prototype.removeCssStyle = function () {
        var stylesheet = null;
        for (var i in document.styleSheets) {
            if (document.styleSheets[i].href && document.styleSheets[i].href.indexOf("forguncyPluginBootstrap.min.css") !== -1) {
                stylesheet = document.styleSheets[i];
                break;
            }
        }
        if (stylesheet != null) {
            stylesheet.deleteRule(stylesheet.cssRules.length - 1);
        }
    }

    CarouselCellType.prototype.getIndicators = function (length) {
        var indicators_ol = $("<ol></ol>");
        indicators_ol.addClass("carousel-indicators");
        for (var i = 0; i < length; i++) {
            var li = $("<li></li>");
            li.attr("data-target", "#" + this.ID);
            li.attr("data-slide-to", i);
            li.css("border", "1px solid " + this.foreColor);
            if (i === 0) {
                li.addClass("active");
            }

            indicators_ol.append(li);
        }

        var li_widths = 12 * length + 2;
        indicators_ol.css("left", "calc(50% - " + Math.floor(li_widths / 2) + "px)");

        return indicators_ol;
    }

    CarouselCellType.prototype.getWrapperForSlides = function (imageInfos, showCaptions, isBinding) {
        var wrapper_div = $("<div></div>");
        wrapper_div.addClass("carousel-inner");
        wrapper_div.attr("role", "listbox");
        wrapper_div.css("width", "100%");
        wrapper_div.css("height", "100%");
        for (var i = 0; i < imageInfos.length; i++) {
            var div = $("<div></div>");
            div.addClass("item");
            div.css("width", "100%");
            div.css("height", "100%");
            if (i === 0) {
                div.addClass("active");
            }

            var image = this.getImageToDiv(imageInfos[i], isBinding);
            div.append(image);

            if (showCaptions) {
                var captionsDiv = this.getCaptionsDiv(imageInfos[i], this.foreColor);
                div.append(captionsDiv);
            }

            if (imageInfos[i].CommandList) {
                div.on("click", (function (commandList, self) {
                    return function (e) {
                        self.excuteCommand(commandList);
                    };
                })(imageInfos[i].CommandList, this));

                div.css("cursor", "pointer");
            }

            wrapper_div.append(div);
        }

        return wrapper_div;
    }

    CarouselCellType.prototype.getImageToDiv = function (imageInfo, isBinding) {
        var imageDiv = $("<div></div>");

        var imagePath = this.GetImagePath(imageInfo.ImagePath, isBinding);
        if (imagePath != null && imagePath !== "") {
            imageDiv.css("background-image", "url('" + imagePath + "')");
        }
        
        imageDiv.css("background-position", "center");
        imageDiv.css("background-size", "contain");
        imageDiv.css("background-repeat", "no-repeat");
        imageDiv.css("width", "100%");
        imageDiv.css("height", "100%");

        return imageDiv;
    }

    CarouselCellType.prototype.getCaptionsDiv = function (imageInfo) {
        var caption_div = $("<div></div>");
        caption_div.addClass("carousel-caption");
        caption_div.css("color", this.foreColor);

        if (imageInfo.Caption && imageInfo.Caption !== "") {
            var caption_h3 = $("<h3></h3>");
            $(caption_h3).text(imageInfo.Caption);

            caption_h3.css("text-overflow", "ellipsis");
            caption_h3.css("overflow", "hidden");
            caption_div.append(caption_h3);
        }

        if (imageInfo.Description && imageInfo.Description !== "") {
            var caption_p = $("<p></p>");
            $(caption_p).text(imageInfo.Description);

            caption_p.css("text-overflow", "ellipsis");
            caption_p.css("overflow", "hidden");
            caption_div.append(caption_p);
        }

        return caption_div;
    }

    CarouselCellType.prototype.getLeftRightControls = function (leftOrRight) {
        var control_a = $("<a></a>");
        control_a.addClass("carousel-control " + leftOrRight);
        if (leftOrRight === "left") {
            control_a.attr("data-slide", "prev");
        } else {
            control_a.attr("data-slide", "next");
        }
        control_a.attr("role", "button");
        control_a.attr("href", "#" + this.ID);

        var span = $("<span></span>");
        span.addClass("glyphicon glyphicon-chevron-" + leftOrRight);
        span.attr("aria-hidden", "true");

        var span2_text = leftOrRight === "left" ? "Previous" : "Next";
        var span2 = $("<span>" + span2_text + "</span>");
        span2.addClass("sr-only");

        control_a.append(span);
        control_a.append(span2);

        return control_a;
    }

    CarouselCellType.prototype.GetImagePath = function (image, isBinding) {
        if (image) {
            if (isBinding) {
                if (image.startsWith("data:image/png;base64")) {
                    return image;
                }
                return Forguncy.Helper.SpecialPath.getUploadImageFolderPathInServer() + image;
            }
            return Forguncy.Helper.SpecialPath.getUploadFileFolderPathInDesigner() + "CarouselCellType/Images/" + image;
        }
        return null;
    }

    CarouselCellType.prototype.onLoad = function () {
        var element = this.CellElement;
        var cellTypeMetaData = element.CellType;

        if (!cellTypeMetaData.IsBinding) {
            this.initCarouselDiv(cellTypeMetaData.ImageInfos, cellTypeMetaData);
            this.hasLoadImages = true;
            this.startCarousel();
        }
    }

    CarouselCellType.prototype.startCarousel = function () {
        var element = this.CellElement;
        var cellTypeMetaData = element.CellType;
        var interval = false;
        if (cellTypeMetaData.AutoSlide && cellTypeMetaData.Interval > 0) {
            interval = cellTypeMetaData.Interval * 1000;
        }

        var pauseWhenHover = false;
        if (cellTypeMetaData.AutoSlide && cellTypeMetaData.PauseWhenHover) {
            pauseWhenHover = "hover";
        }

        $('#' + this.id).carousel({
            interval: interval,
            pause: pauseWhenHover,
            wrap: cellTypeMetaData.Wrap === true ? true : false
        });
    }

    CarouselCellType.prototype.destroy = function () {
        var element = this.CellElement;
        var cellTypeMetaData = element.CellType;
        if (cellTypeMetaData.ShowIndicators) {
            this.removeCssStyle();
        }
    }

    CarouselCellType.prototype.getValueFromElement = function () {
        return null;
    }

    CarouselCellType.prototype.setValueToElement = function (element, value) {

    }

    CarouselCellType.prototype.disable = function () {
        _super.prototype.disable.call(this);
    }

    CarouselCellType.prototype.enable = function () {
        _super.prototype.enable.call(this);
    }

    return CarouselCellType;
}(Forguncy.CellTypeBase));

// Key format is "Namespace.ClassName, AssemblyName"
Forguncy.CellTypeHelper.registerCellType("CarouselCellType.Carousel, CarouselCellType", CarouselCellType);