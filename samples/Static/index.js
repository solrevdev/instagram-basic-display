//
//  You can edit these variables as long as you have read the above
//  ---------------------------------------------------------------
//
const client_id = 'your-client-id-here';
const redirect_url = 'your-https-redirect-url-here';

//
//  should not need to edit below
//  -----------------------------
//

async function getReadmeContent() {
  const url = `/readme.md?nonce=${btoa(+new Date()).slice(-7, -2)}`;
  const response = await fetch(url, {method: 'GET'});
  const result = await response.text();
  //console.log('result', result);
  return result;
}

async function displayReadme() {
  const content = await getReadmeContent();
  const md = window.markdownit();
  const readme = document.getElementById('readme');
  readme.innerHTML = md.render(content);
}

loginButton.onclick = function(e) {
  loginButton.href = `https://api.instagram.com/oauth/authorize?client_id=${client_id}&redirect_uri=${redirect_url}&scope=user_profile,user_media&response_type=code`;
  return true;
};

(function() {
  displayReadme();
  console.log('/samples/Static/index.js loaded');
})();
