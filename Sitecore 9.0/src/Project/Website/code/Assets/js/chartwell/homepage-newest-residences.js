// JavaScript Slick customization - Home Newest Residences
var newestResidencesData = [
  {
    content: {
      en: {
        link: '/continuum-of-care/chartwell-waterford-retirement-community',
        heading: 'Chartwell Waterford Retirement Residence',
        image: '-/media/images/newest-residences/Waterford.jpg',
      },
      fr: {
        link: '/fr/complexes-évolutifs/complexe-pour-retraités-chartwell-waterford',
        heading: 'Complexe pour retraités Chartwell Waterford',
        image: '-/media/images/newest-residences/Waterford.jpg',
      },
      address: '2160 Baronwood Drive, Oakville, ON, K6H 7E5',
      telephone: '289 644-2950'
    }      
  },
  {
    content: {
      en: {
        link: '/retirement-residences/chartwell-le-montcalm-residence-pour-retraites/overview',
        heading: 'Chartwell Le Montcalm résidence pour retraités',
        image: '-/media/images/newest-residences/Montcalm.jpg',
      },
      fr: {
        link: '/fr/résidences-pour-retraités/chartwell-le-montcalm-résidence-pour-retraités/aperçu',
        heading: 'Chartwell Le Montcalm résidence pour retraités',
        image: '-/media/images/newest-residences/Montcalm.jpg',
      },
      address: '95, Montcalm Nord, Candiac, QC, J5R 0R8',
      telephone: '579-886-8891'
    }
  },
  {
    content: {
      en: {
        link: '/the-sumach',
        heading: 'The Sumach by Chartwell',
        image: '-/media/images/newest-residences/Sumach.jpg',
      },
      fr: {
        link: '/the-sumach',
        heading: 'The Sumach by Chartwell',
        image: '-/media/images/newest-residences/Sumach.jpg',
      },
      address: '500 Dundas Street East, Toronto, ON, M5A 2B4',
      telephone: '416-910-3431'
    }
  },
  {
    content: {
      en: {
        link: '/retirement-residences/chartwell-le-st-gabriel-residence-pour-retraites/overview',
        heading: 'Chartwell Le St-Gabriel résidence pour retraités',
        image: '-/media/images/newest-residences/LeST_GABRIEL.jpg',
      },
      fr: {
        link: '/fr/résidences-pour-retraités/chartwell-le-st-gabriel-résidence-pour-retraités/aperçu',
        heading: 'Chartwell Le St-Gabriel résidence pour retraités',
        image: '-/media/images/newest-residences/LeST_GABRIEL.jpg',
      },
      address: '5885, chemin de Chambly, Longueuil, QC, J3Y 0T9',
      telephone: '579 880-8910'
    }
  },
  {
    content: {
      en: {
        link: '/retirement-residences/chartwell-wescott-retirement-residence/overview',
        heading: 'Chartwell Wescott Retirement Residence',
        image: '-/media/images/newest-residences/Wescott.jpg',
      },
      fr: {
        link: '/fr/résidences-pour-retraités/chartwell-wescott-retirement-residence/aperçu',
        heading: 'Chartwell Wescott Retirement Residence',
        image: '-/media/images/newest-residences/Wescott.jpg',
      },
      address: '3841 Allan Drive SW, Edmonton, AB,  T6W 0S7',
      telephone: '587-487-4032'
    }
  },
  {
    content: {
      en: {
        link: '/continuum-of-care/chartwell-carlton-retirement-community',
        heading: 'Chartwell Carlton Retirement Residence',
        image: '-/media/images/newest-residences/Carlton.jpg',
      },
      fr: {
        link: '/fr/complexes-évolutifs/complexe-pour-retraités-chartwell-carlton',
        heading: 'Chartwell Carlton Retirement Residence',
        image: '-/media/images/newest-residences/Carlton.jpg',
      },
      address: '4110 Norfolk Street, Burnaby, BC, V5G 1E8',
      telephone: '778-300-2388'
    }
  }
];

for (var c = 0; c < newestResidencesData.length; c++) {
  var current = newestResidencesData[c];
  var carouselMarkup = '<div style="background-image:url(' + current.content[ChartwellGlobalLang].image + ');"><span class="descriptionNewRes">'
    + '<h3><a href="' + current.content[ChartwellGlobalLang].link + '" target="_blank">'
    + current.content[ChartwellGlobalLang].heading 
    + '<span class="glyphicon glyphicon-link colorGold" aria-hidden="true"></span>'
    + '</a></h3>'
    + '<p>'
    + '<a href="' + current.content[ChartwellGlobalLang].link + '" target="_blank"><span class="colorGold glyphicon glyphicon-map-marker" aria-hidden="true"></span>'
    + current.content.address
    + '</a>'
    + '<a href="tel:' + current.content.telephone + '"><span class="colorGold glyphicon glyphicon-phone-alt" aria-hidden="true"></span>'
    + current.content.telephone
    + '</a>'
    + '</p>'
    + '</span></div>';
  var carouselNavMarkup = '<div style="background-image:url(' + current.content[ChartwellGlobalLang].image + ');"></div>';
  $('#newestResidences').append(carouselMarkup);
  $('#newestResidenceNav').append(carouselNavMarkup);
}

$('.newestResidences').slick({
  slidesToShow: 1,
  slidesToScroll: 1,
  arrows: true,
  fade: true,
  asNavFor: '.newestResidences-nav',
  mobileFirst: true,
  autoplay: true,
  autoplaySpeed: 3000,
});
$('.newestResidences-nav').slick({
  slidesToShow: 5,
  slidesToScroll: 1,
  asNavFor: '.newestResidences',
  dots: true,
  centerMode: true,
  focusOnSelect: true,
  responsive: [{breakpoint: 1024, settings: {slidesToShow: 3, slidesToScroll: 3, infinite: true, dots: true}}]
});
