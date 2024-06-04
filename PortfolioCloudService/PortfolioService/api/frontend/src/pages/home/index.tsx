import React from 'react';
import Navbar from '../../components/navbar/navbar';

const Home = (): React.JSX.Element => {
  return (
    <div>
      <Navbar />
      <div className="home-page">
        <div className="columns ">

          <div className="column">
            <a style={{ marginLeft: 120 }} href="/">
              <img
                src="home2.png"
                width={600}
                loading="lazy"
                alt=""
              />
            </a>
          </div>
          <div className="column" style={{ marginLeft: -250 }}>
            <h1 style={{ fontSize: 62, marginTop: 60 }}>The future of money is here</h1>
            <h2>We're the most trusted place for people and businesses to buy, sell, and manage crypto.</h2>

            <div className="field mt-2">
              <div className="control" style={{ marginTop: 300 }}>
                <h2>Explore crypto like Bitcoin, Ethereum, and Dogecoin.</h2>
                <button className="button is-outlined" onClick={() => { window.location.href = "/registration" }} style={{ marginTop: 10 }}>
                  Buy now
                </button>
              </div>
            </div>
          </div>

        </div>
        <div className="home-addon" style={{ marginLeft: 40, marginRight: 40 }}>
          <h1>Simply and securely buy, sell, and manage hundreds of cryptocurrencies.</h1>
          <br />

          <div className='columns'>
            <div className='column' style={{ backgroundColor: 'white', padding: 20, margin: 15, paddingLeft: 50, paddingRight: 50, borderRadius: 15 }}>
              <a href="/portfolio">
                <img
                  src="bit.png"
                  loading="lazy"
                  width={100}
                  alt=""
                />
              </a>
              <h2>Bitcoin</h2>
              <h3>$4,531,403.30</h3>
            </div>

            <div className='column' style={{ backgroundColor: 'white', padding: 20, margin: 15, paddingLeft: 50, paddingRight: 50, borderRadius: 15 }}>
              <a href="/portfolio">
                <img
                  src="eth.png"
                  loading="lazy"
                  width={100}
                  alt=""
                />
              </a>
              <h2>Etheteum</h2>
              <h3>$267,287.57</h3>
            </div>

            <div className='column' style={{ backgroundColor: 'white', padding: 20, margin: 15, paddingLeft: 50, paddingRight: 50, borderRadius: 15 }}>
              <a href="/portfolio">
                <img
                  src="teth.png"
                  loading="lazy"
                  width={100}
                  alt=""
                />
              </a>
              <h2>Tether</h2>
              <h3>$106.93</h3>
            </div>

            <div className='column' style={{ backgroundColor: 'white', padding: 20, margin: 15, paddingLeft: 50, paddingRight: 50, borderRadius: 15 }}>
              <a href="/portfolio">
                <img
                  src="sol.png"
                  loading="lazy"
                  width={100}
                  alt=""
                />
              </a>
              <h2>Solana</h2>
              <h3>$10,222.85</h3>
            </div>

            <div className='column' style={{ backgroundColor: 'white', padding: 20, margin: 15, paddingLeft: 50, paddingRight: 50, borderRadius: 15 }}>
              
                  <a href="/portfolio">
                    <img
                      src="xrp.png"
                      loading="lazy"
                      width={100}
                      alt=""
                    />
                  </a>
                  <h2>XRP</h2>
                  <h3>$62.23</h3>
            </div>

            <div className='column is-narow' style={{ backgroundColor: 'white', padding: 20, margin: 15, paddingLeft: 50, paddingRight: 50, borderRadius: 15 }}>
              <a href="/portfolio">
                <img
                  src="usdc.png"
                  loading="lazy"
                  width={100}
                  alt=""
                />
              </a>
              <h2>USDC</h2>
              <h3>$106.98</h3>
            </div>
          </div>
        </div>
      </div>
    </div >
  );
}

export default Home;