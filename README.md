# Meet TTF-Engine

TTF-E is a dynamic rule engine used to calculate which premium matches best with a certain parameters. Rules are based on a DSL called 'ttfspec'. This allows business analysts to reconfigure the entire server without intervention of dev. The engine is exposed by a single http endpoint (api/calculator), future development could add restfull access to the rules, to allow adding / removing rules on the fly. This prototype does not include authentication of any kind, and should only be used for educational purposes.

## Usage

The solution contains 2 runnable projects:
* `ttf-web` is a .Net Core webapp, which defaults to localhost/. 
* `ttf-cli` is a .Net Core console application, using self-hosting to host the App. (change appconfig.json to alter hostname / port)

To test the API, use following URI pattern: http://localhost/api/calculator/base/110/?d=1&e=2&f=3
Where 'base' is the name of the spec you want to use, and 110 is the boolean combination of a, b & c.
                                                                            
Both solutions are based on the NancyFx web framework, which depends on (auto-discovered) modules to serve the content.

## Configuration

### Output

The engine can be run in 3 different modes, each modifying the output of the requests:
* `EngineConfig.Mode = EngineOutputMode.AllMatchingRules`
Using this mode will return all matches that can be made for a given input. When multiple calculations are defines per X (S, R, T) then all possible calculations will be returned.
* `EngineConfig.Mode = EngineOutputMode.AllMatchingRulesHighestPrioCalculation`
This mode is similar to AllMatchingRules, but will only return one output per X. Only the calculation added last will be used for a given X. A certain input can still lead to multiple outputs, when more then one X is possible.
* `EngineConfig.Mode = EngineOutputMode.FirstMatchingRuleOnly`
The only mode that will always return just one result. This will be the first match found for X, using the first X-calculation match.

### ttfspec
Basic ttfspec files are pretty simple to create:
```
A && B && !C => X = S
A && B && C => X = R
!A && B && C => X = T

X = S => Y = D + (D * E / 100)
X = R => Y = D + (D * (E - F) / 100)
X = T => Y = D - (D * F / 100)
```
Whitespace usage does not influence the ttf engine parser. It's possible to have inheritance with ttfspec files:
```
inherits base
A && B && !C => X = T
A && !B && C => X = S

X = S => Y = F + D + (D * E / 100)
``` 
When using inheritance, ttfspec files higher up the chain will always have higher priority the the base 'files'.
NOTE: the filename of the ttfspec file will be used as Engine name, and will determine the eventual URL.
