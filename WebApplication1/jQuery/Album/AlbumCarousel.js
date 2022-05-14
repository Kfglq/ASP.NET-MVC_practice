const { Carousel } = require("bootstrap");

var _imgNum = $('#count').val(),
    _currectNum = 0,
    _carouselBlcok,
    _carouselContenter,
    _carouselControl,
    _carouseTimer,
    _carouseInterval = 5000,
    _carouseAnimateTime = 700,
    _carouseWidth = 800,
    _carouseIsAnimate = false;

$(function () {
    _carouselBlcok = $('.myCarousel');
    _carouselContenter = _carouselBlcok.find('.carousel_container');
    _carouselControl = _carouselBlcok.find('.carousel_control');
    _carouselContenter.width(_carouseWidth * _imgNum).css({ left: 0 });
    _carouselControl.find('li').eq(_currectNum).addClass('active');
    _carouseTimer = setTimeout(CarouselToNext, _carouseInterval);
    _carouseIsAnimate = false;
    $('li', _carouselControl).click(function () {
        CarouselTo($(this).index());
    });
    $('.carousel_previous', _carouselBlcok).click(function () {
        CarouselTo(_currectNum - 1);
    });
    $('.carousel_next', _carouselBlcok).click(function () {
        CarouselTo(_currectNum + 1);
    });
    function CarouselToNext() {
        CarouselTo(_currectNum + 1);
    }
    function CarouselTo(ind) {
        if (_carouseIsAnimate)
            return false;
        _carouseIsAnimate = true;
        clearTimeout(_carouseTimer);
        _carouselContenter.css({ left: -_currectNum * _carouseWidth + 'px' });
        if (ind < 0) {
            ind = _imgNum - 1;
        }
        else if (ind >= _imgNum) {
            ind = 0;
        }
        _currectNum = ind;
        _carouselContenter.animate({
            left: -_currectNum * _carouseWidth + 'px'
        },
        _carouseAnimateTime,
        function () {
            _carouselControl.find('li.active').removeClass('active');
            _carouselControl.find('li').eq(_currectNum).addClass('active');
            _carouseIsAnimate = false;
            _carouseTimer = setTimeout(CarouselToNext, _carouseInterval);
        });
    }
});