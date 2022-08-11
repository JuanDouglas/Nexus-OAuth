This is an API entirely made and managed by [Nexus Company](https://nexus-company.tech).

If you want to use an API to authenticate your application use the following routes:
- /api/OAuth/AccessToken
- /api/Accounts/MyAccount
> These routes are available in the [OAuth](#operations-tag-OAuth) and [Accounts](#operations-tag-Accounts) area.

## Authenticated Request 
Post OAuth authorization use your access token on "Authorization" header for routes where a authentication is required.
> The header format should be as follows: "{Token Type} {Access Token}" 
> example: "Barear AAAAAAAAAAAAAAAAAAA"

### Example 
This example show one http request for get user account.

			POST /api/Accounts/MyAccount HTTP/1.1
			Host: auth.nexus-company.tech
			Authorization: Barear AAAAAAAAAAAAAAAA
			User-Agent: Example Web Site

			-- EMPTY BODY (FOR EXAMPLE)