//
//  🚨 PLEASE NOTE
//  ---------------
//
//  This example shows how to authenticate and consume the Instagram Basic Display API but please do be aware that
//  because it is static and client side you should not publish direct to production as it does not protect
//  your `client_secret` variable.

//
//  You can edit these variables as long as you have read the above
//  ---------------------------------------------------------------
//
const client_id = 'your-client-id-here';
const client_secret = 'your-client-secret-here';
const redirect_url = 'your-https-redirect-url-here';

//
//  the html wrapper around an Instagram image. [[caption]] will be replaced at runtime with the real caption and [[url]]
//  will be replaced with the real media_url. also see styles.css for how the .thumbnail class is styled.
//
const imageTemplate = ` <div class="thumbnail">
                          <img
                            alt="[[caption]]"
                            src="[[url]]"
                            class="img"
                            height="150"
                            width="150" />
                        </div>`;

//
//  You should not need to edit below this line.
//  -------------------------------------------
//
const photosDiv = document.getElementById('photos');
const loadMoreButton = document.getElementById('load-more');
const pleaseWaitDialog = $('#pleaseWaitDialog');
const urlParams = new URLSearchParams(window.location.search);

const code = urlParams.get('code');
const state = urlParams.get('state');

let access_token = '';
let loadMorePhotos = true;
let isLoading = false;
let apiUrl = '';
let photos = [];

if (code) {
  pageLoad();
  hidePleaseWait();

  loadMoreButton.addEventListener('click', handleLoadMoreClick);
  window.addEventListener('scroll', _.throttle(handleScroll, 1000, {trailing: true, leading: true}));
} else {
  window.location.href = '/index.html';
}

console.log('/samples/static/oauth.js loaded');

async function pageLoad() {
  await swapAuthCodeForAccessToken();
  await getInstagramMedia();
}

async function swapAuthCodeForAccessToken() {
  try {
    const url = `https://api.instagram.com/oauth/access_token`;

    const data = new URLSearchParams();
    data.append('client_id', client_id);
    data.append('client_secret', client_secret);
    data.append('grant_type', 'authorization_code');
    data.append('redirect_uri', redirect_url);
    data.append('code', code);

    const response = await fetch(url, {method: 'POST', body: data});
    if (response.ok) {
      const json = await response.json();
      access_token = json.access_token;
      return json.access_token;
    } else {
      window.location.href = '/index.html';
    }
  } catch (error) {
    console.error(error);
  }
}

async function getInstagramMedia() {
  try {
    if (apiUrl === '') {
      apiUrl = `https://graph.instagram.com/me/media?fields=caption,id,media_type,media_url,permalink,thumbnail_url,username,timestamp,children.id,children.media_type,children.media_url,children.permalink,children.thumbnail_url,children.username,children.timestamp&access_token=${access_token}`;
    }

    showPleaseWait();

    const response = await fetch(apiUrl, {method: 'GET'});
    const result = await response.json();
    //console.log('result', result);

    if (result.error) {
      console.log(`getInstagramMedia - API Error from ${apiUrl} : ${result.error}`);
      hidePleaseWait();
    } else {
      // filter out albums and videos for now
      const data = result.data.filter(item => item.media_type === 'IMAGE');
      renderPhotos(data);

      if (result.paging && result.paging.next && result.paging.next !== apiUrl) {
        apiUrl = result.paging.next;
      } else {
        apiUrl = '';
        loadMorePhotos = false;
      }

      hidePleaseWait();
    }
  } catch (error) {
    console.error(error);
    hidePleaseWait();
  }
}

function showPleaseWait() {
  loadMoreButton.disabled = true;
  isLoading = true;
  pleaseWaitDialog.modal();
}

function hidePleaseWait() {
  loadMoreButton.disabled = false;
  isLoading = false;
  pleaseWaitDialog.modal('hide');
}

function buildImage(photo) {
  const img = imageTemplate.replace('[[caption]]', photo.caption).replace('[[url]]', photo.media_url);
  return img;
}

function renderPhotos(data) {
  photos.push.apply(photos, data);
  photosDiv.innerHTML = '';

  photos.forEach(photo => {
    if (photosDiv) {
      photosDiv.innerHTML += buildImage(photo);
    }
  });
}

function handleScroll(event) {
  let bottomOfWindow = false;
  const totalHeight = window.innerHeight + window.pageYOffset;
  const scrollHeight = document.body.offsetHeight - 250;

  if (totalHeight >= scrollHeight) {
    bottomOfWindow = true;
  } else {
    bottomOfWindow = false;
  }

  if (bottomOfWindow) {
    if (loadMorePhotos && !isLoading) {
      getInstagramMedia();
    }
  }
}

function handleLoadMoreClick(event) {
  event.preventDefault();

  if (loadMorePhotos && !isLoading) {
    getInstagramMedia();
  }
}
