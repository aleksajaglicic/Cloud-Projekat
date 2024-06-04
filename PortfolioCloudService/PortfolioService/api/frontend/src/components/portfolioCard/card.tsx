import React from "react";
import { ICardProps } from "../../interfaces/props/ICardProps";
import ISummary from "../../interfaces/ISummary";
import axios, { AxiosResponse } from "axios";

const Card: React.FC<ICardProps> = ({ title, user_id, type }) => {
  const defaultData: ISummary = {
    id: 0,
    user_id: "",
    type: 'profit',
    summary: 0.0,
    net_worth: 0.0
  }
  const [summary, setSummary] = React.useState<ISummary>(defaultData);

  React.useEffect(() => {
    const fetchSummary = async () => {
      try {
        if (user_id === null) return;

        const response: AxiosResponse = await axios.post('http://127.0.0.1:10100/api/Portfolio/get_summary_by_user_id', { user_id: user_id }, {
          headers: {
            'Content-Type': 'application/json',
          },
        });

        if (response.status === 200) {
          setSummary(response.data);
        }
      }
      catch { }
    }

    fetchSummary();

    const intervalId = setInterval(fetchSummary, 3000);

    // Cleanup the interval on component unmount
    return () => clearInterval(intervalId);
  }, [user_id])

  return (
    <div className="column">
      <div className="card">
        <div className="card-content">
          <div className="columns is-vcentered is-hcentered">
            <div className="column">
              <p className="title">{title}</p>
              {summary.user_id === null ?
                <div>
                  <div
                    style={{
                      display: "flex",
                      alignItems: "center",
                      justifyContent: "center",
                      background: 'transparent'
                    }}
                  >
                    <div
                      style={{
                        width: "50px",
                        height: "50px",
                        border: "6px solid transparent",
                        borderTop: "6px solid #0E7490",
                        borderRadius: "50%",
                        animation: "spin 2s linear infinite",
                      }}
                    ></div>
                  </div>
                </div>
                :
                <div>
                  {type === "net" && summary && summary.net_worth &&
                    <div className="columns">
                      <div className="column" style={{ marginTop: -10 }}>
                        <span className={`subtitle ${summary?.net_worth < 0.0 ? 'has-text-danger' : 'has-text-success'}`}>
                          {"$" + (summary?.net_worth ? summary.net_worth.toFixed(2).toString() : "0.0")}
                        </span>
                      </div>
                      <div className="column is-narrow is-flex is-align-items-center is-justify-content-center">
                        <img
                          className={`ml-2 ${summary?.net_worth < 0.0 ? 'has-text-danger' : 'has-text-success'}`}
                          src={summary?.net_worth < 0.0 ? "gubitak.png" : "dobit.png"}
                          style={{ marginTop: -70 }}
                          width={80} // Adjust the width as needed
                          height={80} // Adjust the height as needed
                          alt=""
                        />
                      </div>
                    </div>
                  }
                  {type === "sum" && summary && summary.summary &&
                    <div className="columns">
                      <div className="column" style={{ marginTop: -10 }}>
                        <span className={`subtitle ${summary?.summary < 0.0 ? 'has-text-danger' : 'has-text-success'}`}>
                          {(summary?.summary ? summary.summary.toFixed(2).toString() : "0.0") + "%"}
                        </span>
                      </div>
                      <div className="column is-narrow is-flex is-align-items-center is-justify-content-center">
                        <img
                          className={`ml-2 ${summary?.summary < 0.0 ? 'has-text-danger' : 'has-text-success'}`}
                          src={summary?.summary < 0.0 ? "gubitak.png" : "dobit.png"}
                          style={{ marginTop: -70 }}
                          width={80} // Adjust the width as needed
                          height={80} // Adjust the height as needed
                          alt=""
                        />
                      </div>
                    </div>
                  }
                </div>
              }
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Card;
